$(document).ready(function () {
    // Handle Login Form Submission
    $("#loginForm").submit(function (e) {
        e.preventDefault(); // Prevent default form submission

        $.ajax({
            url: "/Account/Login", // ✅ FIXED: Correct URL for login
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Email: $("#loginEmail").val(),
                Password: $("#loginPassword").val()
            }),
            success: function (response) {
                if (response.success) {
                    alert("Login successful! Redirecting...");
                    window.location.href = response.redirectUrl; // ✅ Redirect to correct dashboard
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                alert(xhr.responseText); // Show error message
            }
        });
    });

    // Handle Sign Up Form Submission
    $("#signupForm").submit(function (e) {
        e.preventDefault(); // Prevent default form submission

        $.ajax({
            url: "/Account/Register",  // ✅ FIXED: Correct controller route
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                FullName: $("#signupFullName").val(),
                Email: $("#signupEmail").val(),
                Password: $("#signupPassword").val(),
                Role: $("#signupRole").val() // Ensure this field exists
            }),
            success: function (response) {
                if (response.success) {
                    alert("Registration successful! Redirecting...");
                    window.location.href = "/Home/Index"; // Redirect to homepage
                } else {
                    alert(response.message); // Show error message
                }
            },
            error: function (xhr) {
                alert(xhr.responseText); // Show server error response
            }
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const forgotPasswordForm = document.getElementById("forgotPasswordForm");

    forgotPasswordForm.addEventListener("submit", function (event) {
        event.preventDefault();
        const email = document.getElementById("email").value;
        const successAlert = document.getElementById("forgotPasswordSuccess");
        const errorAlert = document.getElementById("forgotPasswordError");

        fetch('/Account/ForgotPassword', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    successAlert.classList.remove("d-none");
                    errorAlert.classList.add("d-none");
                } else {
                    errorAlert.textContent = data.message || "Something went wrong.";
                    errorAlert.classList.remove("d-none");
                    successAlert.classList.add("d-none");
                }
            })
            .catch(error => {
                errorAlert.textContent = "An error occurred. Please try again later.";
                errorAlert.classList.remove("d-none");
                successAlert.classList.add("d-none");
            });
    });
});
