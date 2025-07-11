import { get_current_token, invalidate_token, set_token } from "/js/utils.mjs";
document.addEventListener("DOMContentLoaded", function () {
    const token = get_current_token();
    if (token != null) {
        invalidate_token(token);
    }

    const form = document.getElementById("form");
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        const email = document.getElementById("email").value;
        fetch(`/api/users/resetpassword?email=${encodeURIComponent(email)}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        }).then(res => {
            if (!res.ok) {
                console.error("Request failed with status:", res.status);
                console.error("Response text:", res.statusText);
                console.error("Response headers:", res.headers);
                throw new Error("Request failed");
            }

            return res.json();
        }).then(data => {
            if (data && data.message) {
                alert(data.message);
            } else {
                console.error("Invalid response data:", data);
                alert("Invalid response from server");
            }
        }).catch(err => {
            console.error("Error:", err);
            alert("An error occurred while processing your request.");
        });
    });
});
