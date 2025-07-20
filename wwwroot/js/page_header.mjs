import { get_current_token, invalidate_token, set_token } from "/js/utils.mjs";

function set_login_user_ui() {
    let header_menu_item_logout = document.getElementById('header_menu_item_logout');
    if (header_menu_item_logout != null) {
        header_menu_item_logout.removeAttribute('hidden');
        header_menu_item_logout.addEventListener('click', function (evt) {
            evt.preventDefault();
            invalidate_token();
            window.location.reload();
        })
    }
    let header_menu_item_login = document.getElementById('header_menu_item_login');
    if (header_menu_item_login != null) { header_menu_item_login.setAttribute('hidden', ''); }
    let header_profile_menu_item = document.getElementById('header_profile_menu_item');
    if (header_profile_menu_item != null) {
        header_profile_menu_item.removeAttribute('hidden');
    }
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
                set_token(token); // Ensure token is set in localStorage and cookies
                set_login_user_ui();
            })
            .catch(() => {
                console.error(arguments);
                console.error("Token is invalid or expired");
                // Token invalid, show login form
                invalidate_token();
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
