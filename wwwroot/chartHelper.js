 window.renderCategoryChart = (labels, values) => {

    const ctx = document.getElementById('spendingChart').getContext('2d');

    const colors = labels.map((_, i) => {
        const hue = (i * 360 / labels.length);
        return `hsl(${hue}, 70%, 60%)`;
    });

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: colors
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top'
                }
            }
        }
    });

};

