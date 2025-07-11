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
