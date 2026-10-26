const SeatAllocation = {

    init() {

        $("#btnContinue").click(() => {

            this.save();

        });

        $("#btnBack").click(() => {

            history.back();

        });

    },

    save() {

        let rows = [];

        let valid = true;

        $(".seat-category").each(function () {

            let category = $(this).val();

            if (!category) {

                valid = false;

                return;

            }

            rows.push({

                screenRowId: $(this).data("rowid"),

                seatCategoryId: parseInt(category)

            });

        });

        if (!valid) {

            alert("Please assign categories to every row.");

            return;

        }

        fetch(window.location.pathname + "?handler=SaveAllocation", {

            method: "POST",

            headers: {

                "Content-Type": "application/json"

            },

            body: JSON.stringify(rows)

        })
            .then(async response => {

                if (!response.ok) {

                    alert("Unable to save seat allocation.");

                    return;

                }

                const ScreenId = new URLSearchParams(window.location.search).get("ScreenId");

                window.location.href =
                    "/TheaterAdmin/Screen/PreviewScreen?ScreenId=" + ScreenId;


            })

    }

};

$(function () {

    SeatAllocation.init();

});