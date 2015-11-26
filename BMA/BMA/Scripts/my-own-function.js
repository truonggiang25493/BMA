
function formatCurrency(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + " ₫";
}

function formatNumber(x) {
    x = x.toString().split(".").join("");
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}