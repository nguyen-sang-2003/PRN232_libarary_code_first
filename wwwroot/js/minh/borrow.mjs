import { get_current_token, invalidate_token, set_token } from "/js/utils.mjs";

document.addEventListener("DOMContentLoaded", function() {
    const submit_button = document.getElementById("borrow_button");
    console.log(submit_button);

    if(submit_button != null){
        submit_button.addEventListener("click", function(e) {
            console.log("click");
            e.preventDefault();

            const token = get_current_token();

            const book_id = document.getElementById("book_id_data").value;

            fetch(`/api/borrowing/rentals/request?bookId=${book_id}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                }
            })
            .then(res => {
                if (!res.ok) {
                    console.error("Failed to borrow book:", res.statusText);
                    console.error("Response status:", res.status);
                    console.error("Response headers:", res.headers);
                    console.error("Response body:", res.body);
                    throw new Error("Failed to borrow book");
                }
            })
            .then(res => {
                console.log(res);
            })
            .catch(err => {
                console.error(err);
                alert("Error borrowing book: " + err.message);
            });
        });
    }
});

window.attachBorrowListener = function() {
    const submit_button = document.getElementById("borrow_button");
    if(submit_button != null){
        // Remove any previous event listeners to avoid duplicates
        submit_button.replaceWith(submit_button.cloneNode(true));
        const new_button = document.getElementById("borrow_button");
        new_button.addEventListener("click", function(e) {
            console.log("click");
            e.preventDefault();

            const token = get_current_token();
            const book_id = document.getElementById("book_id_data").value;

            fetch(`/api/borrowing/rentals/request?bookId=${book_id}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                }
            })
            .then(res => {
                if (!res.ok) {
                    throw new Error("Failed to borrow book");
                }
                return res.json();
            })
            .then(data => {
                console.log("Borrow result:", data);
                alert("Mượn sách thành công!");
            })
            .catch(err => {
                console.error(err);
                alert("Error borrowing book: " + err.message);
            });
        });
    }
}
