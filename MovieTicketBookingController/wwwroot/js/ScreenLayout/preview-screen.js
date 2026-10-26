const PreviewScreen = {

    layout: window.screenLayout,

    init() {

        if (!this.layout) {

            alert("Layout not found.");

            return;

        }

        this.render();

        $("#btnBack").click(() => history.back());

        $("#btnCreate").click(() => this.finish());

    },

    render() {

        let html = "";

        this.layout.rows.forEach(row => {

            html += this.renderRow(row);

        });

        $("#previewContainer").html(html);

    },

    renderRow(row) {

        if (row.rowType === "EmptyWalkway") {

            return `
                <div class="preview-row">
                    <div class="emptyWalkway">
                        EMPTY WALKWAY
                    </div>
                </div>
            `;
        }

        let seats = "";

        row.seats.forEach(seat => {

            let css = "previewSeat";

            if (seat.seatType === "Walkway") {

                css += " walkway";

            }
            else {

                switch (row.seatCategoryId) {

                    case 1:
                        css += " gold";
                        break;

                    case 2:
                        css += " platinum";
                        break;

                    case 3:
                        css += " recliner";
                        break;

                    default:
                        css += " defaultSeat";
                        break;
                }

            }

            seats += `
                <div class="${css}">
                    ${seat.seatType === "Walkway" ? "" : row.rowName + seat.seatLabel}
                </div>
            `;

        });

        return `
            <div class="row mb-3">

                <div class="col-1 fw-bold">
                    ${row.rowName}
                </div>

                <div class="col">

                    <div class="seatRow">

                        ${seats}

                    </div>

                </div>

            </div>
        `;
    },

    finish() {

        window.location.href = "/TheaterAdmin/Screens";

    }

};

$(function () {

    PreviewScreen.init();

});