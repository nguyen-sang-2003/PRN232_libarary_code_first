document.addEventListener("DOMContentLoaded", function() {
    const submit_button = document.getElementById("submit_button");

    if(submit_button != null){
        submit_button.addEventListener("click", function(e) {
            e.preventDefault();
            const token = document.getElementById("token").value;
            const password = document.getElementById("newPassword").value;

            fetch("/api/users/resetpassword", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ token, password })
            })
            .then(res => {
                if (!res.ok) {
                    console.error("Failed to reset password:", res.statusText);
                    console.error("Response status:", res.status);
                    console.error("Response headers:", res.headers);
                    console.error("Response body:", res.body);
                    throw new Error("Failed to reset password");
                }

                return res.json();
            })
            .then(data => {
                alert("Password reset successfully");
                window.location.href = "/login"; // Redirect to login page
            })
            .catch(err => {
                console.error(err);
                alert("Error resetting password: " + err.message);
            });
        });
    }
});
