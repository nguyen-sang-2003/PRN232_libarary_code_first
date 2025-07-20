import { get_current_token, invalidate_token, set_token } from "/js/utils.mjs";

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
            if (!data || !data.access_token) {
                console.error("Invalid response data:", data);
                alert("Invalid response from server");
                throw new Error("Invalid response from server");
            }

            console.log("Login successful, token received:", data.access_token);
            set_token(data.access_token);
            // check for return_url in request path
            const urlParams = new URLSearchParams(window.location.search);
            const returnUrl = urlParams.get('return_url');
            if (returnUrl) {
                // Redirect to the return URL if it exists
                window.location.href = returnUrl;
                return;
            }
            // If no return URL, redirect to home page
            // Redirect after login
            window.location.href = "/";
        })
        .catch(err => {
            alert("Login failed");
        });
}

document.addEventListener("DOMContentLoaded", function () {
    const token = get_current_token();
    if (token) {
        const urlParams = new URLSearchParams(window.location.search);
        const returnUrl = urlParams.get('return_url');
        if (returnUrl) {
            // Redirect to the return URL if it exists
            window.location.href = returnUrl;
            return;
        }

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
                set_token(token); // Ensure token is set in localStorage and cookies
            })
            .catch(() => {
                console.error(arguments);
                console.error("Token is invalid or expired");
                // Token invalid, show login form
                invalidate_token();
                document.getElementById("user-info").style.display = "none";
                document.getElementById("login-form").style.display = "block";
            });
    }

    document.getElementById("logout-btn").onclick = function () {
        invalidate_token();
        location.reload();
    };

    document.getElementById("login-form").onsubmit = function (e) {
        e.preventDefault();
        on_login_button_click();
    };
});
