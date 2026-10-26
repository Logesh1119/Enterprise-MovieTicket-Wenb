
$(document).ready(function () {
    var selectedDate = new Date();
    var selectedScreenId = 0;
    var screens = [];
    var shows = [];
    var defaultPrices = [];

    initDates();
    loadScreens();

    $("#ddlTheater").on("change", function () {
        loadScreens();
    });

    function initDates() {
        var html = "";
        var d = new Date();
        for (var i = 0; i < 30; i++) {
            var curr = new Date(d);
            curr.setDate(d.getDate() + i);
            var dayNum = curr.getDate();
            var dayName = curr.toLocaleDateString("en-US", { weekday: "short" });
            var dateStr = curr.getFullYear() + "-" + String(curr.getMonth() + 1).padStart(2, "0") + "-" + String(curr.getDate()).padStart(2, "0");
            
            var activeClass = i === 0 ? "active" : "";
            if (i === 0) selectedDate = curr;

            html += `<div class="date-item ${activeClass}" data-date="${dateStr}">
                        <span class="day-number">${dayNum}</span>
                        <span class="day-name">${dayName}</span>
                     </div>`;
        }
        $("#dateSelector").html(html);
    }

    $(document).on("click", ".date-item", function () {
        $(".date-item").removeClass("active");
        $(this).addClass("active");
        selectedDate = new Date($(this).data("date"));
        loadShows();
    });

    function loadScreens() {
        var thId = $("#ddlTheater").val();
        if (!thId) return;

        $.get(`?handler=ScreensAjax&theaterId=${thId}`, function (res) {
            screens = res;
            var html = "";
            
            // Populate screen dropdown in add show form
            var ddlScreens = "";

            $.each(screens, function (i, s) {
                var activeClass = i === 0 ? "active" : "";
                if (i === 0) selectedScreenId = s.id;
                
                html += `<div class="screen-card ${activeClass}" data-id="${s.id}">
                            <h4>${s.name}</h4>
                            <div class="screen-meta">
                                <span>${s.type}</span>
                                <span>${s.audio}</span>
                                <span>${s.seats} Seats</span>
                            </div>
                         </div>`;
                         
                ddlScreens += `<option value="${s.id}">${s.name}</option>`;
            });

            $("#screenList").html(html);
            $("#ddlShowScreen").html(ddlScreens);
            
            loadShows();
            loadMovies();
        });
    }

    $(document).on("click", ".screen-card", function () {
        $(".screen-card").removeClass("active");
        $(this).addClass("active");
        selectedScreenId = $(this).data("id");
        $("#ddlShowScreen").val(selectedScreenId);
        loadShows();
    });

    $("#txtScreenSearch").on("keyup", function () {
        var v = $(this).val().toLowerCase();
        $(".screen-card").each(function () {
            var txt = $(this).text().toLowerCase();
            $(this).toggle(txt.indexOf(v) > -1);
        });
    });

    function loadShows() {
        if (!selectedScreenId) {
            $("#showList").html("<p>No screen selected.</p>");
            return;
        }

        var dStr = selectedDate.getFullYear() + "-" + String(selectedDate.getMonth() + 1).padStart(2, "0") + "-" + String(selectedDate.getDate()).padStart(2, "0");

        $.get(`?handler=ShowsAjax&screenId=${selectedScreenId}&date=${dStr}`, function (res) {
            shows = res;
            renderShows(shows);
        });
    }

    function renderShows(data) {
        if (!data || data.length === 0) {
            $("#showList").html("<p>No shows scheduled for this date.</p>");
            return;
        }

        var html = "";
        $.each(data, function (i, s) {
            var timeStr = s.showTime.substring(0, 5); // Simplistic format
            var img = s.movieImagePath ? s.movieImagePath : "/uploads/UI/no-image.png";
            html += `
                <div class="show-card">
                    <div class="show-time">${timeStr}</div>
                    <div class="movie-info">
                        <img src="${img}" class="movie-poster" />
                        <div class="movie-details">
                            <h4>${s.movieName}</h4>
                            <div class="movie-tags">
                                <span class="tag">${s.language}</span>
                                <span class="tag">${s.format}</span>
                                <span class="tag">${s.status}</span>
                            </div>
                        </div>
                    </div>
                    <div class="show-price">${s.priceRange}</div>
                    <div class="show-bookings">${s.bookings} Bookings</div>
                    <div class="show-actions">
                        <button class="btn-edit" data-id="${s.id}">Edit</button>
                        <button class="btn-danger btn-delete" data-id="${s.id}">Delete</button>
                    </div>
                </div>
            `;
        });
        $("#showList").html(html);
    }

    $("#txtShowSearch").on("keyup", function () {
        var v = $(this).val().toLowerCase();
        $(".show-card").each(function () {
            var txt = $(this).text().toLowerCase();
            $(this).toggle(txt.indexOf(v) > -1);
        });
    });

    function loadMovies() {
        var thId = $("#ddlTheater").val();
        $.get(`?handler=MoviesAjax&theaterId=${thId}`, function (res) {
            var html = "";
            $.each(res, function (i, m) {
                html += `<option value="${m.id}">${m.name}</option>`;
            });
            $("#ddlShowMovie").html(html);
        });
    }

    // Drawer Logic
    $("#btnAddShow").on("click", function () {
        $("#drawerTitle").text("Add new show");
        $("#hdnShowId").val(0);
        
        // Setup Date
        var dStr = selectedDate.getFullYear() + "-" + String(selectedDate.getMonth() + 1).padStart(2, "0") + "-" + String(selectedDate.getDate()).padStart(2, "0");
        $("#txtShowDate").val(dStr);
        $("#txtShowTime").val("09:00");
        
        // Fetch categories for pricing
        $.get(`?handler=DefaultPricesAjax&screenId=${selectedScreenId}`, function(prices) {
            defaultPrices = prices;
            var html = "";
            $.each(prices, function (i, p) {
                html += `
                    <div class="price-config-row" data-cat="${p.seatCategoryId}">
                        <span>${p.categoryName}</span>
                        <input type="number" class="form-control txt-price" style="width:120px;" placeholder="Price" value="${p.price}" />
                    </div>
                `;
            });
            $("#pricingContainer").html(html);
            
            $("#drawerOverlay").addClass("is-active");
            $("#showDrawer").addClass("is-open");
        });
    });

    $("#btnCloseDrawer, #btnCancelShow, #drawerOverlay").on("click", function () {
        $("#showDrawer").removeClass("is-open");
        $("#drawerOverlay").removeClass("is-active");
    });

    $("#btnSaveShow").on("click", function () {
        var id = parseInt($("#hdnShowId").val());
        
        var prices = [];
        $(".price-config-row").each(function () {
            prices.push({
                seatCategoryId: $(this).data("cat"),
                price: parseFloat($(this).find(".txt-price").val()) || 0
            });
        });

        var payload = {
            id: id,
            screenId: parseInt($("#ddlShowScreen").val()),
            movieId: parseInt($("#ddlShowMovie").val()),
            language: $("#ddlLanguage").val(),
            format: $("#ddlFormat").val(),
            showDate: $("#txtShowDate").val(),
            showTime: $("#txtShowTime").val() + ":00",
            status: $("#ddlShowStatus").val(),
            prices: prices
        };

        var handler = id > 0 ? "UpdateShowAjax" : "CreateShowAjax";

        $.ajax({
            url: `?handler=${handler}`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function(res) {
                if (res && res.success) {
                    $("#showDrawer").removeClass("is-open");
                    $("#drawerOverlay").removeClass("is-active");
                    loadShows();
                } else {
                    alert("Error saving show.");
                }
            }
        });
    });

    $(document).on("click", ".btn-edit", function () {
        var showId = $(this).data("id");
        $.get(`?handler=ShowForEditAjax&showId=${showId}`, function(show) {
            $("#drawerTitle").text("Edit show");
            $("#hdnShowId").val(show.id);
            $("#ddlShowScreen").val(show.screenId);
            $("#ddlShowMovie").val(show.movieId);
            $("#ddlLanguage").val(show.language);
            $("#ddlFormat").val(show.format);
            $("#txtShowDate").val(show.showDate.substring(0, 10));
            $("#txtShowTime").val(show.showTime.substring(0, 5));
            $("#ddlShowStatus").val(show.status);

            var html = "";
            $.each(show.prices, function (i, p) {
                html += `
                    <div class="price-config-row" data-cat="${p.seatCategoryId}">
                        <span>${p.categoryName}</span>
                        <input type="number" class="form-control txt-price" style="width:120px;" placeholder="Price" value="${p.price}" />
                    </div>
                `;
            });
            $("#pricingContainer").html(html);

            $("#drawerOverlay").addClass("is-active");
            $("#showDrawer").addClass("is-open");
        });
    });

    $(document).on("click", ".btn-delete", function () {
        if (confirm("Are you sure you want to delete this show?")) {
            var showId = $(this).data("id");
            $.post(`?handler=DeleteShowAjax&showId=${showId}`, function(res) {
                if (res && res.success) {
                    loadShows();
                } else {
                    alert("Error deleting show.");
                }
            });
        }
    });
});

