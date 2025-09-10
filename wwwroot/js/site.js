

// Function to update notification badges
function updateBadges() {
    // In a real application, you would fetch this data from an API
    // For demonstration, we'll use static values
    const unreadNotifications = 3;
    const pendingRequests = 2;
    
    if (unreadNotifications > 0) {
        $('#notificationsBadge').text(unreadNotifications).show();
    } else {
        $('#notificationsBadge').hide();
    }
    
    if (pendingRequests > 0) {
        $('#requestsCountBadge').text(pendingRequests).show();
    } else {
        $('#requestsCountBadge').hide();
    }
}

// Initialize when document is ready
$(document).ready(function() {
    updateBadges();
    
    // Simulate real-time updates
    setInterval(updateBadges, 30000);
});// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
