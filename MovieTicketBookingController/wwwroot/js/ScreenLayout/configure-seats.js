const ScreenConfigurator = {

    screenLayout: {

        screenId: 0,

        totalRows: 0,

        totalSeats: 0,

        rows: []

    },

    selectedRowIndex: null,

    selectedSeats: [],



    init() {

        const screenId = Number($("#ScreenId").val());

        if (window.screenLayout &&
            (window.screenLayout.rows || window.screenLayout.Rows)) {

            const data = window.screenLayout;

            this.screenLayout = {

                screenId: data.ScreenId,

                totalRows: data.TotalRows,

                totalSeats: data.TotalSeats,

                rows: (data.Rows || []).map(row => ({

                    id: row.Id,

                    rowName: row.RowName,

                    rowType: row.RowType,

                    totalSeats: row.TotalSeats,

                    walkwayCount: row.WalkwayCount,

                    seatCategoryId: row.SeatCategoryId,

                    displayOrder: row.DisplayOrder,

                    isReverse: row.IsReverse,

                    confirmed: true,   // already configured

                    seats: (row.Seats || []).map(seat => ({

                        seatNo: seat.SeatNo,

                        seatLabel: seat.SeatLabel,

                        seatType: seat.SeatType,

                        selected: false

                    }))

                }))

            };
            this.screenLayout.rows.forEach(row => {

                row.seats.forEach(seat => {

                    if (seat.selected === undefined) {

                        seat.selected = false;

                    }

                });

            });

        }
        else {

            this.screenLayout = {

                screenId: ScreenId,

                totalRows: 0,

                totalSeats: 0,

                rows: []

            };

        }

        this.calculateTotals();

        this.bindEvents();

        this.render();

        this.setNextRowName();

    },

    setNextRowName() {

        $("#txtRowName").val(
            this.getRowName(this.screenLayout.rows.length)
        );

    },


    getRowName(index) {

        let name = "";

        index++;

        while (index > 0) {

            index--;

            name = String.fromCharCode(65 + (index % 26)) + name;

            index = Math.floor(index / 26);

        }

        return name;

    },

    clearForm() {

        $("#ddlType").val("Seats");

        $("#txtSeats").val("");

        $("#txtWalkway").val("");

        this.setNextRowName();
        this.clearError();
    },

    showError(message) {

        $("#validationBox")
            .removeClass("d-none")
            .text(message);

    },

    clearError() {

        $("#validationBox")
            .addClass("d-none")
            .text("");


    },
    validateRow() {

        const rowType = $("#ddlType").val();

        const totalSeats = Number($("#txtSeats").val());

        const walkway = Number($("#txtWalkway").val());

        if (rowType == "Seats") {

            if (!totalSeats || totalSeats <= 0) {

                this.showError("Enter total seats.");

                return false;

            }

            if (walkway < 0) {

                this.showError("Invalid walkway count.");

                return false;

            }

            if (walkway >= totalSeats) {

                this.showError("Walkway cannot be greater than seats.");

                return false;

            }

        }

        this.clearError();

        return true;

    },
    addRow() {

        if (!this.validateRow())
            return;

        const row = {

            rowName: $("#txtRowName").val(),

            rowType: $("#ddlType").val(),

            totalSeats: Number($("#txtSeats").val()),

            walkwayCount: Number($("#txtWalkway").val()),

            seatCategoryId: null,

            isReverse: false,

            confirmed: false,

            displayOrder: this.screenLayout.rows.length + 1,

            seats: []

        };

        if (row.rowType == "Seats") {

            for (let i = 1; i <= row.totalSeats; i++) {

                row.seats.push({

                    seatNo: i,

                    seatLabel: row.rowName + i,

                    seatType: "Seat",

                    selected: false

                });

            }

        }

        this.screenLayout.rows.push(row);
        this.calculateTotals();
        this.render();

        this.clearForm();

    },
    toggleSeat(e) {

        const rowIndex = Number($(e.currentTarget).data("row"));
        const seatIndex = Number($(e.currentTarget).data("seat"));

        const row = this.screenLayout.rows[rowIndex];

        if (row.confirmed)
            return;

        const seat = row.seats[seatIndex];

        if (seat.seatType === "Walkway")
            return;

        seat.selected = !seat.selected;
        this.calculateTotals();
        this.render();

    },
    render() {

        let html = "";

        this.screenLayout.rows.forEach((row, index) => {

            html += this.renderRow(row, index);

        });

        $("#layoutContainer").html(html);

        const canContinue = this.screenLayout.rows.every(r =>
            r.rowType === "EmptyWalkway" || r.confirmed
        );

        $("#btnContinue").prop("disabled", !canContinue);

    },
    renderRow(row, index) {

        // Handle Empty Walkway rows first
        if (row.rowType === "EmptyWalkway") {

            return `

<div class="seat-config-card mb-4">

    <div class="d-flex justify-content-between align-items-center">

        <strong>Row ${row.rowName}</strong>

        <div class="d-flex gap-2">

            <button
                class="btn btn-outline-secondary btnCopyRow"
                data-row="${index}">
                📄
            </button>

            <button
                class="btn btn-outline-danger btnDeleteRow"
                data-row="${index}">
                🗑
            </button>

        </div>

    </div>

    <div class="empty-walkway-row mt-3">
        WALKWAY
    </div>

</div>

`;

        }


        let seats = "";

        let i = 0;

        while (i < row.seats.length) {

            const seat = row.seats[i];

            if (seat.seatType === "Walkway") {

                let walkwaySeats = 0;

                while (
                    i < row.seats.length &&
                    row.seats[i].seatType === "Walkway"
                ) {
                    walkwaySeats++;
                    i++;
                }

                seats += `
            <div class="seat-aisle"
                 style="width:${walkwaySeats * 42}px;">
            </div>
        `;

                continue;
            }

            seats += `
        <button
            class="seat-box ${seat.selected ? "selected-seat" : ""}"
            data-row="${index}"
            data-seat="${i}">
            ${seat.seatLabel}
        </button>
    `;

            i++;

        }


        return `

<div class="seat-config-card mb-4">

<div class="d-flex justify-content-between">

<div>

<strong>

Row ${row.rowName}

</strong>

</div>

<div class="d-flex gap-2">

    <button
        class="btn btn-outline-primary btnRefreshRow"
        data-row="${index}"
        title="Reset">
        ↻
    </button>

    <button
        class="btn btn-success btnConfirmRow"
        data-row="${index}"
        title="Confirm">
        ✓
    </button>

    <button
        class="btn btn-outline-secondary btnCopyRow"
        data-row="${index}"
        title="Copy">
        📄
    </button>

    <button
        class="btn btn-outline-danger btnDeleteRow"
        data-row="${index}"
        title="Delete">
        🗑
    </button>

</div>

</div>

<div class="seat-container mt-3">

${seats}

</div>

</div>

`;

    },
    copyRow(e) {

        const rowIndex = Number($(e.currentTarget).data("row"));

        const originalRow = this.screenLayout.rows[rowIndex];

        const newRow = structuredClone(originalRow);

        newRow.rowName = this.getRowName(this.screenLayout.rows.length);

        newRow.displayOrder = this.screenLayout.rows.length + 1;

        newRow.confirmed = originalRow.confirmed;

        newRow.seats.forEach((seat, index) => {

            seat.selected = false;
            seat.seatLabel = newRow.rowName + (index + 1);

        });

        this.screenLayout.rows.push(newRow);
        this.calculateTotals();
        this.render();

        this.setNextRowName();

    },
    deleteRow(e) {

        const rowIndex = Number($(e.currentTarget).data("row"));

        if (!confirm("Delete this row?"))
            return;

        this.screenLayout.rows.splice(rowIndex, 1);

        this.reArrangeRows();

        this.calculateTotals();
        this.render();

        this.setNextRowName();


    },

    reArrangeRows() {

        this.screenLayout.rows.forEach((row, index) => {

            row.rowName = this.getRowName(index);

            row.displayOrder = index + 1;

            row.seats.forEach((seat, seatIndex) => {

                seat.seatLabel = row.rowName + seat.seatNo;

            });

        });

    },


    confirmRow(e) {

        const rowIndex = Number($(e.currentTarget).data("row"));

        const row = this.screenLayout.rows[rowIndex];

        if (row.rowType !== "Seats")
            return;

        const selectedSeats = row.seats.filter(s => s.selected);

        if (selectedSeats.length !== row.walkwayCount) {

            this.showError(`Please select exactly ${row.walkwayCount} walkway seats.`);

            return;

        }
        selectedSeats.forEach(seat => {

            seat.selected = false;
            seat.seatType = "Walkway";

        });

        // Keep the seat order intact.
        // Rendering will collapse consecutive walkway seats
        // into a single aisle automatically.

        row.confirmed = true;

        this.clearError();
        this.calculateTotals();
        this.render();

    },


    resetRow(e) {

        const rowIndex = Number($(e.currentTarget).data("row"));

        const row = this.screenLayout.rows[rowIndex];

        row.confirmed = false;

        row.seats.forEach(seat => {

            seat.selected = false;

            if (seat.seatType === "Walkway") {

                seat.seatType = "Seat";

            }

        });

        this.clearError();

        this.calculateTotals();

        this.render();

    },
    calculateTotals() {

        this.screenLayout.totalRows =
            this.screenLayout.rows.length;

        this.screenLayout.totalSeats =
            this.screenLayout.rows.reduce((total, row) => {

                return total +
                    row.seats.filter(s => s.seatType === "Seat").length;

            }, 0);

    },
    async continue() {

        try {

            const response = await fetch(
                window.location.pathname + "?handler=SaveLayout",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                        // "RequestVerification": $('input[name="__RequestVerificationToken"]').val()
                    },
                    body: JSON.stringify(this.screenLayout)
                });

            if (!response.ok) {

                const errorText = await response.text();

                console.error("SERVER ERROR:", errorText);

                this.showError(errorText);

                return;
            }

            const result = await response.json();

            if (result.success) {

                window.location.href =
                    "/TheaterAdmin/Screen/AllocateSeats?ScreenId=" +
                    this.screenLayout.screenId;
            }
            else {

                this.showError("Unable to save layout.");
            }
        }
        catch (error) {

            console.error(error);

            this.showError("Something went wrong while saving.");
        }
    },

    bindEvents() {

        $("#btnSubmit").on("click", () => this.addRow());

        $("#btnContinue").on("click", () => this.continue());

        $("#btnClear").on("click", () => this.clearForm());

        $(document).on("click", ".seat-box",
            e => this.toggleSeat(e));

        $(document).on("click", ".btnConfirmRow",
            e => this.confirmRow(e));

        $(document).on("click", ".btnRefreshRow",
            e => this.resetRow(e));

        $(document).on("click", ".btnCopyRow",
            e => this.copyRow(e));

        $(document).on("click", ".btnDeleteRow",
            e => this.deleteRow(e));

    }

};

$(function () {

    ScreenConfigurator.init();

});