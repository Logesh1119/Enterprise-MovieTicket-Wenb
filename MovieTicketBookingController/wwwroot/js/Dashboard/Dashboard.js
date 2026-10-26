$(document).ready(function () {
    initChart();

    function initChart() {
        $.ajax({
            url: "?handler=RefreshAjax",
            type: "GET",
            success: function (data) {
                if (data && data.chartData) {
                    renderChart(data.chartData);
                }
            },
            error: function () {
                console.error("Failed to load dashboard chart data.");
            }
        });
    }

    function renderChart(chartData) {
        var ctx = document.getElementById('bookingChart').getContext('2d');
        
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: chartData.labels,
                datasets: [
                    {
                        label: 'Online',
                        data: chartData.onlineBookings,
                        backgroundColor: 'rgba(220, 53, 69, 0.8)',
                        borderColor: '#dc3545',
                        borderWidth: 1,
                        borderRadius: 4
                    },
                    {
                        label: 'On-site',
                        data: chartData.onSiteBookings,
                        backgroundColor: 'rgba(52, 58, 64, 0.8)',
                        borderColor: '#343a40',
                        borderWidth: 1,
                        borderRadius: 4
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: {
                    mode: 'index',
                    intersect: false,
                },
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            usePointStyle: true,
                            boxWidth: 8
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.raw !== undefined && context.raw !== null) {
                                    label += '₹' + parseFloat(context.raw).toLocaleString('en-IN');
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            borderDash: [5, 5],
                            color: '#e9ecef'
                        },
                        ticks: {
                            callback: function(value, index, values) {
                                if (value >= 1000) {
                                    return '₹' + value / 1000 + 'k';
                                }
                                return '₹' + value;
                            }
                        }
                    }
                }
            }
        });
    }
});
