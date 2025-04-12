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

$(document).ready(function () {
    $('#forgotPasswordForm').on('submit', function (e) {
        e.preventDefault();

        var email = $('#forgotEmail').val();

        if (!email) {
            alert("Please enter your email.");
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Account/ForgotPassword',
            contentType: 'application/json',
            data: JSON.stringify({ Email: email }),
            success: function (response) {
                alert("Reset link sent! Please check your email.");
                $('#forgotPasswordModal').modal('hide');
            },
            error: function (xhr) {
                var response = xhr.responseJSON;
                alert(response?.message || "Something went wrong.");
            }
        });
    });
});

window.addEventListener("DOMContentLoaded", function () {
    const params = new URLSearchParams(window.location.search);
    const token = params.get("token");
    const email = params.get("email");

    if (token && email) {
        // Set values inside hidden fields
        document.getElementById("resetToken").value = token;
        document.getElementById("resetEmail").value = email;

        // Show modal
        const resetModal = new bootstrap.Modal(document.getElementById('resetPasswordModal'));
        resetModal.show();
    }
});

// Handle Reset Password Submit
document.getElementById("resetPasswordForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const data = {
        Token: document.getElementById("resetToken").value,
        Email: document.getElementById("resetEmail").value,
        NewPassword: document.getElementById("newPassword").value,
        ConfirmPassword: document.getElementById("confirmPassword").value
    };

    fetch("/Account/ResetPassword", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: new URLSearchParams(data)
    })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                alert("Password reset successful!");
                location.href = "/"; // redirect to homepage or login
            } else {
                alert("Error: " + (result.message || "Something went wrong."));
            }
        });
});