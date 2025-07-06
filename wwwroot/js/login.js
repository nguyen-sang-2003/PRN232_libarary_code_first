function get_current_token(){
    let token_from_localstorage = localStorage.getItem("token");
    let token_from_cookies = document.cookie.split('; ').find(row => row.startsWith('token='));
    token_from_cookies = token_from_cookies ? token_from_cookies.split('=')[1] : null;
    if (token_from_localstorage) {
        return token_from_localstorage;
    } else if (token_from_cookies) {
        return token_from_cookies;
    } else {
        return null;
    }
}

function invalidate_token() {
    localStorage.removeItem("token");
    document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    console.log("Token invalidated");
}


function on_login_button_click() {
    // TODO show loading state
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    fetch("/api/getaccesstoken", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    })
    .then(res => {
        if (!res.ok) throw new Error("Login failed");
        return res.json();
    })
    .then(data => {
        localStorage.setItem("token", data.access_token);
        // Redirect after login
        window.location.href = "/";
    })
    .catch(err => {
        alert("Login failed");
    });
}

function on_register_button_click() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    if (username && password) {
        fetch("/api/users/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        })
        .then(res => {
            if (!res.ok) {
                // log all information about the response
                console.error("Registration failed:", res.statusText);
                console.error("Response status:", res.status);
                console.error("Response headers:", res.headers);

                throw new Error("Registration failed");
            }
            return res.json();
        })
        .then(data => {
            alert("Registration successful");
            // send requests to login
            on_login_button_click();
        })
        .catch(err => {
            alert("Registration failed: " + err.message);
        });
    }
}

document.addEventListener("DOMContentLoaded", function() {
    const token = get_current_token();
    if (token) {
        // Fetch user info using token
        fetch("/api/users/info", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        })
        .then(res => {
            if (!res.ok) {
                console.error("Failed to fetch user info:", res.statusText);
                console.error("Response status:", res.status);
                console.error("Response headers:", res.headers);

                throw new Error("Invalid token");
            }
            return res.json();
        })
        .then(user => {
            document.getElementById("user-info").style.display = "block";
            document.getElementById("user-details").innerText = JSON.stringify(user, null, 2);
            document.getElementById("login-form").style.display = "none";
        })
        .catch(() => {
            console.error(arguments);
            console.error("Token is invalid or expired");
            // Token invalid, show login form
            localStorage.removeItem("token");
            document.getElementById("user-info").style.display = "none";
            document.getElementById("login-form").style.display = "block";
        });
    }
    document.getElementById("logout-btn").onclick = function() {
        localStorage.removeItem("token");
        location.reload();
    };

    document.getElementById("login-form").onsubmit = function(e) {
        e.preventDefault();
        on_login_button_click();
    };

    document.getElementById("register-btn").onclick = function() {
        on_register_button_click();
    };
});
