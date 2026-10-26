/* ===========================================================
   SINGLE-PAGE SCREEN CONFIGURATION WIZARD JS (STEP-BY-STEP DB)
=========================================================== */

$(function () {
    'use strict';

    var data = window.serverWizardData || {};

    // Normalize PascalCase properties that come from C# System.Text.Json serialization
    function normalizeRows(rows) {
        if (!rows) return [];
        return rows.map(function (r) {
            return {
                id: r.Id || r.id || 0,
                rowName: r.RowName || r.rowName || '',
                rowType: r.RowType || r.rowType || 'Seats',
                totalSeats: r.TotalSeats || r.totalSeats || 0,
                walkwayCount: r.WalkwayCount || r.walkwayCount || 0,
                seatCategoryId: r.SeatCategoryId || r.seatCategoryId || null,
                displayOrder: r.DisplayOrder || r.displayOrder || 1,
                isReverse: r.IsReverse || r.isReverse || false,
                confirmed: true, // Existing rows from DB are already confirmed
                seats: normalizeSeatList(r.Seats || r.seats || [])
            };
        });
    }

    function normalizeSeatList(seats) {
        return seats.map(function (s) {
            return {
                seatNo: s.SeatNo || s.seatNo || 0,
                seatLabel: s.SeatLabel || s.seatLabel || '',
                seatType: s.SeatType || s.seatType || 'Seat',
                selected: false
            };
        });
    }

    function getCatId(cat) {
        return cat.Id || cat.id;
    }

    function getCatName(cat) {
        return cat.CategoryName || cat.categoryName || '';
    }

    var rawLayout = data.layout || {};
    var rawRows   = rawLayout.Rows || rawLayout.rows || [];

    var wizardState = {
        screenId: data.screenId || 0,
        theaterId: data.theaterId || 0,
        specs: data.specs || {},
        layout: {
            screenId: data.screenId || 0,
            rows: normalizeRows(rawRows)
        },
        categories: data.categories || [],
        currentStep: data.initialStep || 1
    };

    /* Helper: Row Naming (A, B, C ... Z, AA, AB ...) */
    function getRowLetter(index) {
        var name = "";
        index++;
        while (index > 0) {
            index--;
            name = String.fromCharCode(65 + (index % 26)) + name;
            index = Math.floor(index / 26);
        }
        return name;
    }

    /* ─────────────────────────────────────────────────────────
       STEPPER UI & CONTAINER TOGGLING
    ───────────────────────────────────────────────────────── */

    function setStep(stepNum) {
        wizardState.currentStep = stepNum;

        // Stepper Bar UI
        $('.wizard-step-item').each(function () {
            var step = $(this).data('step');
            $(this).removeClass('active completed');
            if (step < stepNum) {
                $(this).addClass('completed');
            } else if (step === stepNum) {
                $(this).addClass('active');
            }
        });

        // Toggle Step Containers
        $('.step-container').removeClass('active');
        $('#step-' + stepNum + '-container').addClass('active');

        // Scroll to top
        window.scrollTo({ top: 0, behavior: 'smooth' });

        // Trigger step-specific renders
        if (stepNum === 2) renderStep2();
        if (stepNum === 3) renderStep3();
        if (stepNum === 4) renderStep4();
    }

    /* ─────────────────────────────────────────────────────────
       STEP 1: SCREEN DETAILS
    ───────────────────────────────────────────────────────── */

    // Amenity chip toggle
    $(document).on('click', '.amenity-chip-item', function () {
        $(this).toggleClass('selected');
    });

    $('#btnStep1Continue').on('click', function () {
        var name = $('#txtScreenName').val().trim();
        var code = $('#txtScreenCode').val().trim();
        if (!name || !code) {
            alert('Please fill in Screen Name and Screen Code.');
            return;
        }

        var selectedAmenIds = [];
        $('.amenity-chip-item.selected').each(function () {
            selectedAmenIds.push(parseInt($(this).data('id')));
        });

        var payload = {
            screenId: wizardState.screenId,
            theaterId: wizardState.theaterId,
            screenName: name,
            screenCode: code,
            floor: $('#txtFloor').val().trim(),
            screenType: $('#ddlScreenType').val(),
            audioTechnology: $('#ddlAudioTechnology').val(),
            seatingType: $('#ddlSeatingType').val(),
            status: $('#ddlStatus').val() === 'true',
            amenId: selectedAmenIds
        };

        var $btn = $(this);
        $btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: '?handler=SaveScreenDetails',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (res) {
                $btn.prop('disabled', false).text('Continue');
                if (res && res.success) {
                    wizardState.screenId = res.screenId;
                    wizardState.layout.screenId = res.screenId;
                    setStep(2);
                } else {
                    alert('Error saving screen details.');
                }
            },
            error: function () {
                $btn.prop('disabled', false).text('Continue');
                alert('Server error saving screen details.');
            }
        });
    });

    /* ─────────────────────────────────────────────────────────
       STEP 2: CONFIGURE SEATS
    ───────────────────────────────────────────────────────── */

    function renderStep2() {
        $('#step2RowName').val(getRowLetter(wizardState.layout.rows.length));
        var html = '';

        $.each(wizardState.layout.rows, function (rIdx, row) {
            html += '<div class="seat-config-row" data-row="' + rIdx + '">';
            html += '<span class="seat-row-label">Row ' + row.rowName + '</span>';
            html += '<div class="seat-row-items">';

            if (row.rowType === 'EmptyWalkway') {
                html += '<span style="font-weight:600; color:#667085; text-transform:uppercase; font-size:12px;">WALKWAY</span>';
            } else {
                $.each(row.seats || [], function (sIdx, seat) {
                    var isWalkway = (seat.seatType === 'Walkway');
                    var isSelected = seat.selected;
                    var cls = 'seat-box-item';
                    if (isWalkway) cls += ' is-walkway';
                    else if (isSelected) cls += ' selected';

                    html += '<div class="' + cls + '" data-row="' + rIdx + '" data-seat="' + sIdx + '">' + (isWalkway ? '' : seat.seatLabel) + '</div>';
                });
            }

            html += '</div>';

            // Row Action buttons
            html += '<div class="row-actions-group">';
            if (row.rowType !== 'EmptyWalkway') {
                html += '<button type="button" class="btn-row-action btn-reset-row" data-row="' + rIdx + '" title="Reset">↻</button>';
                html += '<button type="button" class="btn-row-action btn-row-action--confirm btn-confirm-row" data-row="' + rIdx + '" title="Confirm Walkways">✓</button>';
            }
            html += '<button type="button" class="btn-row-action btn-copy-row" data-row="' + rIdx + '" title="Copy Row">📄</button>';
            html += '<button type="button" class="btn-row-action btn-delete-row" data-row="' + rIdx + '" title="Delete Row">🗑</button>';
            html += '</div>';

            html += '</div>';
        });

        $('#step2LayoutContainer').html(html);
    }

    // Add Row submit
    $('#btnStep2AddRow').on('click', function () {
        var rowType = $('#ddlStep2RowType').val();
        var rowName = $('#step2RowName').val().trim() || getRowLetter(wizardState.layout.rows.length);
        var totalSeats = parseInt($('#txtStep2Seats').val()) || 0;
        var walkwayCount = parseInt($('#txtStep2Walkway').val()) || 0;

        if (rowType === 'Seats' && totalSeats <= 0) {
            alert('Please enter total seats.');
            return;
        }

        var newRow = {
            id: 0,
            rowName: rowName,
            rowType: rowType,
            totalSeats: totalSeats,
            walkwayCount: walkwayCount,
            seatCategoryId: null,
            displayOrder: wizardState.layout.rows.length + 1,
            isReverse: false,
            confirmed: (rowType === 'EmptyWalkway'),
            seats: []
        };

        if (rowType === 'Seats') {
            for (var i = 1; i <= totalSeats; i++) {
                newRow.seats.push({
                    seatNo: i,
                    seatLabel: rowName + i,
                    seatType: 'Seat',
                    selected: false
                });
            }
        }

        wizardState.layout.rows.push(newRow);
        $('#txtStep2Seats').val('');
        $('#txtStep2Walkway').val('');
        renderStep2();
    });

    // Seat click for walkway selection
    $(document).on('click', '.seat-box-item', function () {
        var rIdx = $(this).data('row');
        var sIdx = $(this).data('seat');
        var row = wizardState.layout.rows[rIdx];
        if (!row || row.confirmed) return;

        var seat = row.seats[sIdx];
        if (seat && seat.seatType !== 'Walkway') {
            seat.selected = !seat.selected;
            renderStep2();
        }
    });

    // Confirm Row (convert selected seats to Walkway)
    $(document).on('click', '.btn-confirm-row', function () {
        var rIdx = $(this).data('row');
        var row = wizardState.layout.rows[rIdx];
        if (!row) return;

        $.each(row.seats, function (i, seat) {
            if (seat.selected) {
                seat.seatType = 'Walkway';
                seat.selected = false;
            }
        });
        row.confirmed = true;
        renderStep2();
    });

    // Reset Row
    $(document).on('click', '.btn-reset-row', function () {
        var rIdx = $(this).data('row');
        var row = wizardState.layout.rows[rIdx];
        if (!row) return;

        $.each(row.seats, function (i, seat) {
            seat.seatType = 'Seat';
            seat.selected = false;
        });
        row.confirmed = false;
        renderStep2();
    });

    // Copy Row
    $(document).on('click', '.btn-copy-row', function () {
        var rIdx = $(this).data('row');
        var original = wizardState.layout.rows[rIdx];
        if (!original) return;

        var copy = JSON.parse(JSON.stringify(original));
        copy.id = 0;
        copy.rowName = getRowLetter(wizardState.layout.rows.length);
        copy.displayOrder = wizardState.layout.rows.length + 1;

        $.each(copy.seats, function (i, seat) {
            seat.seatLabel = copy.rowName + seat.seatNo;
        });

        wizardState.layout.rows.push(copy);
        renderStep2();
    });

    // Delete Row
    $(document).on('click', '.btn-delete-row', function () {
        var rIdx = $(this).data('row');
        wizardState.layout.rows.splice(rIdx, 1);

        // Re-index row names
        $.each(wizardState.layout.rows, function (i, r) {
            r.rowName = getRowLetter(i);
            r.displayOrder = i + 1;
            $.each(r.seats, function (sIdx, s) {
                s.seatLabel = r.rowName + s.seatNo;
            });
        });

        renderStep2();
    });

    // Step 2 Continue -> Next Step
    $('#btnStep2Continue').on('click', function () {
        if (!wizardState.layout.rows.length) {
            alert('Please add at least one row.');
            return;
        }
        setStep(3);
    });

    /* ─────────────────────────────────────────────────────────
       STEP 3: ALLOCATE SEATS
    ───────────────────────────────────────────────────────── */

    function renderStep3() {
        var html = '';

        $.each(wizardState.layout.rows, function (rIdx, row) {
            if (row.rowType === 'EmptyWalkway') return;

            html += '<div class="category-row-card">';
            html += '<span class="seat-row-label">Row ' + row.rowName + '</span>';

            html += '<div class="seat-row-items">';
            $.each(row.seats || [], function (sIdx, seat) {
                var isWalkway = (seat.seatType === 'Walkway');
                html += '<div class="seat-box-item ' + (isWalkway ? 'is-walkway' : '') + '">' + (isWalkway ? '' : seat.seatLabel) + '</div>';
            });
            html += '</div>';

            html += '<select class="category-select-dropdown" data-row="' + rIdx + '">';
            html += '<option value="">Select Category</option>';

            $.each(wizardState.categories, function (cIdx, cat) {
                var cId = getCatId(cat);
                var sel = (row.seatCategoryId === cId) ? 'selected' : '';
                html += '<option value="' + cId + '" ' + sel + '>' + getCatName(cat) + '</option>';
            });

            html += '</select>';
            html += '</div>';
        });

        $('#step3CategoryContainer').html(html);
    }

    $(document).on('change', '.category-select-dropdown', function () {
        var rIdx = $(this).data('row');
        var catId = parseInt($(this).val()) || null;
        if (wizardState.layout.rows[rIdx]) {
            wizardState.layout.rows[rIdx].seatCategoryId = catId;
        }
    });

    // Step 3 Continue -> Next Step
    $('#btnStep3Continue').on('click', function () {
        setStep(4);
    });

    /* ─────────────────────────────────────────────────────────
       STEP 4: PREVIEW
    ───────────────────────────────────────────────────────── */

    function renderStep4() {
        var html = '';

        $.each(wizardState.layout.rows, function (rIdx, row) {
            if (row.rowType === 'EmptyWalkway') return;

            var catName = 'Not Allocated';
            $.each(wizardState.categories, function (i, c) {
                var cId = c.id || c.Id;
                if (cId === row.seatCategoryId) {
                    catName = c.categoryName || c.CategoryName;
                }
            });

            html += '<div class="category-row-card">';
            html += '<span class="seat-row-label">Row ' + row.rowName + '</span>';

            html += '<div class="seat-row-items">';
            $.each(row.seats || [], function (sIdx, seat) {
                var isWalkway = (seat.seatType === 'Walkway');
                html += '<div class="seat-box-item ' + (isWalkway ? 'is-walkway' : '') + '">' + (isWalkway ? '' : seat.seatLabel) + '</div>';
            });
            html += '</div>';

            html += '<span class="category-badge-pill">' + catName + '</span>';
            html += '</div>';
        });

        $('#step4PreviewContainer').html(html);
    }

    // Step 4 Finish -> Save Layout and Redirect back to Theater Details
    $('#btnStep4Finish').on('click', function () {
        var $btn = $(this);
        $btn.prop('disabled', true).text('Saving...');

        // Send the complete layout including SeatCategoryId
        $.ajax({
            url: '?handler=SaveScreenLayout',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(wizardState.layout),
            success: function (res) {
                if (res && res.success) {
                    window.location.href = '/TheaterAdmin/Theater/Details?id=' + wizardState.theaterId;
                } else {
                    alert('Error saving screen configuration.');
                    $btn.prop('disabled', false).text('Save / Finish');
                }
            },
            error: function () {
                alert('Server error saving screen configuration.');
                $btn.prop('disabled', false).text('Save / Finish');
            }
        });
    });

    // Back buttons
    $('#btnStep2Back').on('click', function () { setStep(1); });
    $('#btnStep3Back').on('click', function () { setStep(2); });
    $('#btnStep4Back').on('click', function () { setStep(3); });

    // Cancel buttons
    $('.btn-wizard-cancel').on('click', function () {
        if (confirm('Cancel and return to Theater Details?')) {
            window.location.href = '/TheaterAdmin/Theater/Details?id=' + wizardState.theaterId;
        }
    });

    // Initialize initial step
    setStep(wizardState.currentStep);
});
