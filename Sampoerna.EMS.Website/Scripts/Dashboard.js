function ChangeNumber(value) {
    var realValue = value;
    var addOns = "";

    if (value > 9999 && value < 10000000) {
        realValue = (value / 1000);
        addOns = "K";
    }
    else if (value > 9999999) {
        realValue = (value / 1000000);
        addOns = "M";
    }

    var total = parseInt(realValue, 10);
    realValue = ThausandSeperator(total, 0) + addOns;

    return realValue;
}