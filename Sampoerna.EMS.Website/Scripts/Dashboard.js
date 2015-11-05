function ChangeNumber(value) {
    var realValue = value;

    if (value > 9999) {
        realValue = (value / 1000);
    }

    if (value > 9999) {
        realValue = realValue + "K";
    }

    return realValue;
}