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

document.addEventListener("DOMContentLoaded", function () {
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
            // document.getElementById("login-form").style.display = "none";
        })
        .catch(() => {
            console.error(arguments);
            console.error("Token is invalid or expired");
            // Token invalid
            localStorage.removeItem("token");
        });


        document.getElementById("logout-btn").onclick = function() {
            localStorage.removeItem("token");
            location.reload();
        };
    }
});

document.onload = function(){
    const token = get_current_token();
    if (token) {

        fetch("/api/SampleEnityBooks/1", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Request failed");
            }

            return response.json();

        })
        .then(data => {
            console.log("data", data);
            console.log(data);

            // dict
            //{"id":1,"title":"1984","authorId":1,"imageBase64":"","publishedDate":"1949-06-08T00:00:00","bookCopies":null,"author":null}
            let id = data.id;
            let title = data.title;

            document.querySelector("#id").innerHTML = id;
            document.querySelector("#title").innerHTML = title;

            document.querySelector("#data_output").innerHTML = JSON.stringify(data);


        })
        .catch(error => {
            console.error("Error:", error);
        });
    }
}
// web pages -> press button/ timeout 5 secs/ page load -> api requests (js get token from localstorage add to header) -> data -> render/update web pages
document.querySelector("#list_using_localstorage_token").addEventListener("click", function () {
    const token = get_current_token();
    if (!token) {
        console.error("No token found in local storage or cookies.");
        return;
    }

    // single page application
    // empty

    fetch("/api/SampleEnityBooks/1", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("Request failed");
        }

        // local storage/ cookies
        return response.json();
        // let token_obj = response.json();
        // console.log(token_obj);
        // localStorage.setItem("token", token_obj["access_token"]);

    })
    .then(data => {
        console.log("data", data);
        // You can store or use the access token here
        // let token_obj = JSON.parse(data);
        // let token_obj = data;
        console.log(data);
        let id = data.id;
        let title = data.title;

        document.querySelector("#id").innerHTML = id;
        document.querySelector("#title").innerHTML = title;
        document.querySelector("#data_output").innerHTML = JSON.stringify(data);
        // localStorage.setItem("token", token_obj["access_token"]);
    })
    .catch(error => {
        console.error("Error:", error);
    });
});
