
$(document).ready(function () {
    loadAll();

    function loadAll() {
        loadData("Genres", "tblGenres");
        loadData("SeatCategories", "tblSeatCats");
        loadData("Amenities", "tblAmenities");
        loadData("Certificates", "tblCertificates");
    }

    // Tabs
    $(".md-tab").on("click", function() {
        $(".md-tab").removeClass("active");
        $(this).addClass("active");
        $(".md-content").removeClass("active");
        $("#" + $(this).data("target")).addClass("active");
    });

    function loadData(type, tableId) {
        $.get(`?handler=${type}Ajax`, function (data) {
            var html = "";
            $.each(data, function (i, item) {
                var name = item.genreName || item.categoryName || item.amenityName || item.certificateName;
                var colorHtml = "";
                if (type === "SeatCategories") {
                    colorHtml = `<td><span class="color-preview" style="background-color:${item.colorCode}"></span>${item.colorCode}</td>`;
                }

                html += `<tr>
                    <td>${item.id}</td>
                    <td>${name}</td>
                    ${colorHtml}
                    <td>
                        <button class="btn-edit" data-type="${type}" data-id="${item.id}" data-name="${name}" data-color="${item.colorCode || ''}"><i class="fa-solid fa-pen"></i></button>
                        <button class="btn-delete" data-type="${type}" data-id="${item.id}"><i class="fa-solid fa-trash"></i></button>
                    </td>
                </tr>`;
            });
            $("#" + tableId + " tbody").html(html);
        });
    }

    $(".search-box").on("keyup", function() {
        var v = $(this).val().toLowerCase();
        var tableId = $(this).data("table");
        $(`#${tableId} tbody tr`).each(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(v) > -1);
        });
    });

    // Drawer Logic
    $(".btn-add").on("click", function () {
        var type = $(this).data("type");
        $("#hdnType").val(type);
        $("#hdnId").val("0");
        $("#txtName").val("");
        $("#drawerTitle").text("Add " + type);

        if (type === "SeatCategory") {
            $("#grpColor").show();
            $("#txtColor").val("#000000");
        } else {
            $("#grpColor").hide();
        }
        
        openDrawer();
    });

    $(document).on("click", ".btn-edit", function () {
        var type = $(this).data("type").replace("ies", "y").replace("es", "").replace("s", ""); // singularize roughly if needed, actually we pass correct singular in data-type above? Wait, in render it passes plural for type. Let's fix it.
        var pluralType = $(this).data("type");
        var singleType = pluralType === "SeatCategories" ? "SeatCategory" : pluralType === "Amenities" ? "Amenity" : pluralType === "Genres" ? "Genre" : "Certificate";
        
        $("#hdnType").val(singleType);
        $("#hdnId").val($(this).data("id"));
        $("#txtName").val($(this).data("name"));
        $("#drawerTitle").text("Edit " + singleType);

        if (singleType === "SeatCategory") {
            $("#grpColor").show();
            $("#txtColor").val($(this).data("color"));
        } else {
            $("#grpColor").hide();
        }
        
        openDrawer();
    });

    $(document).on("click", ".btn-delete", function () {
        if (confirm("Are you sure you want to delete this item?")) {
            var pluralType = $(this).data("type");
            var singleType = pluralType === "SeatCategories" ? "SeatCategory" : pluralType === "Amenities" ? "Amenity" : pluralType === "Genres" ? "Genre" : "Certificate";
            var id = $(this).data("id");

            $.post(`?handler=Delete${singleType}Ajax&id=${id}`, function(res) {
                if(res.success) {
                    loadData(pluralType, getTableId(pluralType));
                } else {
                    alert("Cannot delete this item as it is currently in use (Referenced by other records).");
                }
            });
        }
    });

    $("#btnSave").on("click", function () {
        var id = parseInt($("#hdnId").val());
        var type = $("#hdnType").val();
        var name = $("#txtName").val();

        if(!name) { alert("Name is required."); return; }

        var payload = { id: id };
        if (type === "Genre") payload.genreName = name;
        else if (type === "SeatCategory") { payload.categoryName = name; payload.colorCode = $("#txtColor").val(); }
        else if (type === "Amenity") payload.amenityName = name;
        else if (type === "Certificate") payload.certificateName = name;

        var handler = id > 0 ? `Update${type}Ajax` : `Create${type}Ajax`;
        var pluralType = type === "SeatCategory" ? "SeatCategories" : type === "Amenity" ? "Amenities" : type + "s";

        $.ajax({
            url: `?handler=${handler}`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function(res) {
                if (res && res.success) {
                    closeDrawer();
                    loadData(pluralType, getTableId(pluralType));
                } else {
                    alert("Error saving item.");
                }
            }
        });
    });

    function getTableId(pluralType) {
        if (pluralType === "Genres") return "tblGenres";
        if (pluralType === "SeatCategories") return "tblSeatCats";
        if (pluralType === "Amenities") return "tblAmenities";
        return "tblCertificates";
    }

    function openDrawer() {
        $("#mdOverlay").addClass("is-active");
        $("#mdDrawer").addClass("is-open");
    }

    function closeDrawer() {
        $("#mdDrawer").removeClass("is-open");
        $("#mdOverlay").removeClass("is-active");
    }

    $("#btnCloseDrawer, #btnCancel, #mdOverlay").on("click", closeDrawer);
});

