export function get_current_token(){
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

export function invalidate_token() {
    localStorage.removeItem("token");
    document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    console.log("Token invalidated");
}

export function set_token(token) {
    localStorage.setItem("token", token);
    // Set the token in cookies as well
    document.cookie = `token=${token}; path=/;`;
    console.log("Token set:", token);
}
