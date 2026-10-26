
$(document).ready(function () {
    
    function showAlert(msg, isSuccess) {
        $("#alertMsg")
            .text(msg)
            .removeClass("alert-success alert-danger")
            .addClass(isSuccess ? "alert-success" : "alert-danger")
            .show();
        
        setTimeout(() => $("#alertMsg").hide(), 5000);
    }

    $("#btnSaveProfile").on("click", function () {
        var payload = {
            userId: parseInt($("#hdnUserId").val()),
            username: $("#txtUsername").val(),
            email: $("#txtEmail").val(),
            phoneNumber: $("#txtPhone").val(),
            country: $("#txtCountry").val()
        };

        if(!payload.username || !payload.email) {
            showAlert("Username and Email are required.", false);
            return;
        }

        $.ajax({
            url: "?handler=UpdateProfileAjax",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function(res) {
                if (res && res.success) {
                    showAlert("Profile updated successfully.", true);
                } else {
                    showAlert("Error updating profile.", false);
                }
            }
        });
    });

    $("#btnChangePassword").on("click", function () {
        var payload = {
            userId: parseInt($("#hdnUserId").val()),
            currentPassword: $("#txtCurrentPassword").val(),
            newPassword: $("#txtNewPassword").val()
        };

        if(!payload.currentPassword || !payload.newPassword) {
            showAlert("Both current and new passwords are required.", false);
            return;
        }

        $.ajax({
            url: "?handler=ChangePasswordAjax",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function(res) {
                if (res && res.success) {
                    showAlert("Password changed successfully.", true);
                    $("#txtCurrentPassword").val("");
                    $("#txtNewPassword").val("");
                } else {
                    showAlert("Failed to change password. Ensure your current password is correct.", false);
                }
            }
        });
    });
});

