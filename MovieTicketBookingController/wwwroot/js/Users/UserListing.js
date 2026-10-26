
$(document).ready(function () {
    var allUsers = [];

    loadUsers();

    function loadUsers() {
        $.get("?handler=UsersAjax", function (data) {
            allUsers = data;
            renderUsers(allUsers);
        });
    }

    function renderUsers(users) {
        var html = "";
        $.each(users, function (i, u) {
            var statusClass = u.status ? "status-active" : "status-inactive";
            var statusText = u.status ? "Active" : "Inactive";
            var dateStr = new Date(u.createdAt).toLocaleDateString();

            html += `<tr>
                <td>${u.username}</td>
                <td>${u.email}</td>
                <td>${u.roleName}</td>
                <td><span class="status-badge ${statusClass}">${statusText}</span></td>
                <td>${dateStr}</td>
                <td>
                    <button class="btn-edit" data-id="${u.id}"><i class="fa-solid fa-pen"></i></button>
                    <button class="btn-delete" data-id="${u.id}"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>`;
        });
        $("#userTableBody").html(html);
    }

    // Filter and Search
    $("#txtSearch, #ddlRoleFilter").on("change keyup", function () {
        var search = $("#txtSearch").val().toLowerCase();
        var roleId = $("#ddlRoleFilter").val();

        var filtered = allUsers.filter(function (u) {
            var matchSearch = u.username.toLowerCase().indexOf(search) > -1 || u.email.toLowerCase().indexOf(search) > -1;
            var matchRole = roleId === "" || u.roleId === roleId;
            return matchSearch && matchRole;
        });

        renderUsers(filtered);
    });

    // Drawer Logic
    $("#btnAddUser").on("click", function () {
        $("#drawerTitle").text("Add new user");
        $("#hdnUserId").val("0");
        $("#txtUsername").val("");
        $("#txtEmail").val("");
        $("#txtPassword").val("");
        $("#passwordGroup").show();
        $("#ddlRole").val($("#ddlRole option:first").val());
        $("#txtPhone").val("");
        $("#txtCountry").val("");
        $("#ddlStatus").val("true");
        
        openDrawer();
    });

    $(document).on("click", ".btn-edit", function () {
        var id = $(this).data("id");
        $.get(`?handler=UserForEditAjax&id=${id}`, function(user) {
            if (user) {
                $("#drawerTitle").text("Edit user");
                $("#hdnUserId").val(user.id);
                $("#txtUsername").val(user.username);
                $("#txtEmail").val(user.email);
                $("#passwordGroup").hide(); // Hide password on edit
                $("#ddlRole").val(user.roleId);
                $("#txtPhone").val(user.phoneNumber);
                $("#txtCountry").val(user.country);
                $("#ddlStatus").val(user.status.toString());
                
                openDrawer();
            }
        });
    });

    $(document).on("click", ".btn-delete", function () {
        if (confirm("Are you sure you want to delete this user?")) {
            var id = $(this).data("id");
            $.post(`?handler=DeleteUserAjax&id=${id}`, function(res) {
                if(res.success) {
                    loadUsers();
                } else {
                    alert("Error deleting user.");
                }
            });
        }
    });

    $("#btnSave").on("click", function () {
        var id = parseInt($("#hdnUserId").val());
        
        var payload = {
            id: id,
            username: $("#txtUsername").val(),
            email: $("#txtEmail").val(),
            password: $("#txtPassword").val(),
            roleId: $("#ddlRole").val(),
            phoneNumber: $("#txtPhone").val(),
            country: $("#txtCountry").val(),
            status: $("#ddlStatus").val() === "true"
        };

        if(!payload.username || !payload.email || (id === 0 && !payload.password)) {
            alert("Please fill required fields.");
            return;
        }

        var handler = id > 0 ? "UpdateUserAjax" : "CreateUserAjax";

        $.ajax({
            url: `?handler=${handler}`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function(res) {
                if (res && res.success) {
                    closeDrawer();
                    loadUsers();
                } else {
                    alert("Error saving user. Email might be duplicate.");
                }
            }
        });
    });

    function openDrawer() {
        $("#drawerOverlay").addClass("is-active");
        $("#userDrawer").addClass("is-open");
    }

    function closeDrawer() {
        $("#userDrawer").removeClass("is-open");
        $("#drawerOverlay").removeClass("is-active");
    }

    $("#btnCloseDrawer, #btnCancel, #drawerOverlay").on("click", closeDrawer);
});

