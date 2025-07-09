
// Toggle Mobile Menu
function toggleMobileMenu() {
    const mobileMenu = document.getElementById('mobileMenu');
    const menuButton = document.getElementById('mobileMenuButton');
    mobileMenu.classList.toggle('hidden');
    menuButton.setAttribute('aria-expanded', !mobileMenu.classList.contains('hidden'));
}

// Mobile Menu Event Listener
document.getElementById('mobileMenuButton').addEventListener('click', toggleMobileMenu);
// Toggle Details Section
function toggleDetails(id) {
    const allDetailSectionIds = [
        'motorcycleDetails',
        'sedanDetails',
        'carDetails',
        'pickupTruckDetails',
        'mpvDetails',
        'fusoTruckDetails'
    ];

    const clickedSection = document.getElementById(id);
    const clickedIcon = document.getElementById(id.replace('Details', 'ToggleIcon'));

    if (clickedSection) {
        clickedSection.classList.toggle('hidden');
        if (clickedIcon) {
            clickedIcon.textContent = clickedSection.classList.contains('hidden') ? '+' : '-';
            clickedIcon.setAttribute('aria-expanded', !clickedSection.classList.contains('hidden'));
        }
    } else {
        document.getElementById('trackResult').textContent = `Error: Section ${id} not found.`;
    }
}

// Track Job Function
function trackJob() {
    const jobId = document.getElementById('jobId').value;
    const trackResult = document.getElementById('trackResult');
    const loadingSpinner = document.getElementById('loadingSpinner');
    if (!jobId) {
        trackResult.textContent = 'Please enter a valid Job ID.';
        return;
    }
    loadingSpinner.classList.remove('hidden');
    setTimeout(() => {
        loadingSpinner.classList.add('hidden');
        trackResult.textContent = `Tracking Job ID: ${jobId} - In Transit`;
    }, 1000);
}

// Vehicle Comparison
function showComparison() {
    const checkboxes = document.querySelectorAll('input[name="compare"]:checked');
    const comparisonTable = document.getElementById('comparisonTable');
    if (checkboxes.length === 0) {
        comparisonTable.innerHTML = '<p class="text-gray-600 dark:text-gray-300">Please select at least one vehicle to compare.</p>';
        comparisonTable.classList.remove('hidden');
        return;
    }

    let table = `
        <table class="w-full text-left border-collapse">
            <thead>
                <tr class="bg-gray-200 dark:bg-gray-600">
                    <th class="p-2">Vehicle</th>
                    <th class="p-2">Weight Limit</th>
                    <th class="p-2">Size (W × L × H)</th>
                    <th class="p-2">Base Fee</th>
                    <th class="p-2">Rate per km</th>
                    <th class="p-2">Rate per kg</th>
                </tr>
            </thead>
            <tbody>
    `;

    const vehicles = {
        motorcycle: {
            name: 'Motorcycle',
            weight: '20 kg',
            size: '50 × 50 × 50 cm',
            baseFee: '150 THB',
            ratePerKm: '10 THB',
            ratePerKg: '5 THB'
        },
        sedan: {
            name: '4-door Sedan',
            weight: '100 kg',
            size: '90 × 100 × 75 cm',
            baseFee: '250 THB',
            ratePerKm: '15 THB',
            ratePerKg: '8 THB'
        },
        car: {
            name: '5-door Car',
            weight: '200 kg',
            size: '115 × 115 × 80 cm',
            baseFee: '300 THB',
            ratePerKm: '18 THB',
            ratePerKg: '10 THB'
        },
        pickupTruck: {
            name: 'Pickup Truck',
            weight: '1,100 kg',
            size: '170 × 180 × 90 cm',
            baseFee: '500 THB',
            ratePerKm: '25 THB',
            ratePerKg: '15 THB'
        },
        mpv: {
            name: 'Multi-purpose Vehicle',
            weight: '300 kg',
            size: '130 × 160 × 80 cm',
            baseFee: '600 THB',
            ratePerKm: '30 THB',
            ratePerKg: '12 THB'
        },
        fusoTruck: {
            name: '6 Wheel Fuso Truck',
            weight: '2,400 kg',
            size: '180 × 300 × 200 cm',
            baseFee: '1000 THB',
            ratePerKm: '50 THB',
            ratePerKg: '20 THB'
        }
    };

    checkboxes.forEach(checkbox => {
        const vehicle = vehicles[checkbox.value];
        if (vehicle) {
            table += `
                <tr class="border-b dark:border-gray-500">
                    <td class="p-2">${vehicle.name}</td>
                    <td class="p-2">${vehicle.weight}</td>
                    <td class="p-2">${vehicle.size}</td>
                    <td class="p-2">${vehicle.baseFee}</td>
                    <td class="p-2">${vehicle.ratePerKm}</td>
                    <td class="p-2">${vehicle.ratePerKg}</td>
                </tr>
            `;
        }
    });

    table += '</tbody></table>';
    comparisonTable.innerHTML = table;
    comparisonTable.classList.remove('hidden');
}