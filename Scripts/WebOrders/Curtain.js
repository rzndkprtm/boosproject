let designIdOri = "3";
let itemAction;
let headerId;
let orderId;
let itemId;
let designId;
let customerId;
let companyId;
let companyDetailId;
let loginId;
let roleAccess;
let priceAccess;

iniCurtain();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindColourType($(this).val());
    bindMounting($(this).val());
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindComponentForm(blindtype, $(this).val());
});

$("#heading").on("change", function () {
    bindTrackType($(this).val());
});

$("#headingb").on("change", function () {
    bindTrackTypeB($(this).val());
});

$("#tracktype").on("change", function () {
    bindTrackColour($(this).val());
});

$("#tracktypeb").on("change", function () {
    bindTrackColourB($(this).val());
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#fabrictypeb").on("change", function () {
    bindFabricColourB($(this).val());
});

$("#trackdraw").on("change", function () {
    visibleControlColourLength(1, $(this).val());
});

$("#trackdrawb").on("change", function () {
    visibleControlColourLength(2, $(this).val());
});

$("#width").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 1, $(this).val());
});

$("#widthb").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 2, $(this).val());
});

$("#drop").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 1, $(this).val());
});

$("#dropb").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 2, $(this).val());
});

function loader(itemAction) {
    return new Promise((resolve) => {
        if (itemAction === "create") {
            document.getElementById("divloader").style.display = "none";
            document.getElementById("divorder").style.display = "";
        }
        resolve();
    });
}

function isError(msg) {
    $("#modalError").modal("show");
    document.getElementById("errorMsg").innerHTML = msg;
}

function getOrderHeader(headerId) {
    return new Promise((resolve, reject) => {
        if (!headerId) return resolve();

        $.ajax({
            type: "POST",
            url: "Method.aspx/GetOrderHeader",
            data: JSON.stringify({ headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: ({ d }) => {
                orderId = d.OrderId || "-";
                customerId = d.CustomerId || "-";
                document.getElementById("orderid").innerText = d.OrderId || "-";
                document.getElementById("ordernumber").innerText = d.OrderNumber || "-";
                document.getElementById("ordername").innerText = d.OrderName || "-";
                resolve(d);
            },
            error: reject
        });
    });
}

function getFormAction(itemAction) {
    return new Promise((resolve) => {
        const pageAction = document.getElementById("pageaction");
        if (!pageAction) {
            resolve();
            return;
        }

        const actionMap = {
            create: "Add Item", edit: "Edit Item",
            view: "View Item", copy: "Copy Item"
        };

        pageAction.innerText = actionMap[itemAction] || "";
        resolve();
    });
}

function getCompanyOrder(headerId) {
    return new Promise((resolve, reject) => {
        companyId = "";

        if (!headerId) {
            resolve();
            return;
        }

        const type = "CompanyOrder";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                companyId = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getCompanyDetailOrder(headerId) {
    return new Promise((resolve, reject) => {
        companyDetailId = "";

        if (!headerId) {
            resolve();
            return;
        }

        const type = "CompanyDetailOrder";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: headerId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                companyDetailId = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getRoleAccess(loginId) {
    return new Promise((resolve, reject) => {
        roleAccess = "";

        if (!loginId) {
            resolve();
            return;
        }

        const type = "RoleAccess";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: loginId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                roleAccess = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getPriceAccess(loginId) {
    return new Promise((resolve, reject) => {
        priceAccess = "";

        if (!loginId) {
            resolve();
            return;
        }

        const type = "CustomerPriceAccess";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: loginId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                priceAccess = response.d.trim();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getDesignName(designType) {
    return new Promise((resolve, reject) => {
        const cardTitle = document.getElementById("cardtitle");
        cardTitle.textContent = "";

        if (!designType) {
            resolve();
            return;
        }

        const type = "DesignName";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: designType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                const designName = response.d.trim();
                cardTitle.textContent = designName;
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getBlindName(blindType) {
    if (!blindType) return;

    const type = "BlindName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: blindType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                resolve(response.d);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindBlindType(designType) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designType) {
            const selectedValue = blindtype.value || "";
            Promise.all([
                bindColourType(selectedValue),
                bindMounting(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "BlindType", companydetailid: companyDetailId, designtype: designType, action: itemAction };
        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    blindtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        blindtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        blindtype.add(option);
                    });

                    if (response.d.length === 1) {
                        blindtype.selectedIndex = 0;
                    }

                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindColourType(selectedValue),
                        bindMounting(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindColourType(selectedValue),
                        bindMounting(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType) {
            const selectedValue = colourtype.value || "";
            Promise.all([
                bindComponentForm(blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ProductName", companydetailid: companyDetailId, blindtype: blindType, tubetype: 0, controltype: "0", action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    colourtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        colourtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        colourtype.add(option);
                    });

                    if (response.d.length === 1) {
                        colourtype.selectedIndex = 0;
                    }

                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        bindComponentForm(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        bindComponentForm(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindMounting(blindType) {
    return new Promise((resolve, reject) => {
        const mounting = document.getElementById("mounting");

        if (!blindType) {
            resolve();
            return;
        }

        const listData = { type: "Mounting", blindtype: blindType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    mounting.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        mounting.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        mounting.add(option);
                    });

                    if (response.d.length === 1) {
                        mounting.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindFabricType(designType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["fabrictype", "fabrictypeb"];
        const bindFunctions = [bindFabricColour, bindFabricColourB];

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        if (!designType) {
            const bindPromises = typeIds.map((id, idx) => {
                const val = document.getElementById(id)?.value || "";
                return bindFunctions[idx](val);
            });
            Promise.all(bindPromises).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "FabricTypeByDesign", designtype: designType, companydetailid: companyDetailId, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    const hasMultiple = response.d.length > 1;

                    response.d.forEach((item, index) => {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;

                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) {
                                if (index === 0 && hasMultiple) {
                                    const defaultOption = document.createElement("option");
                                    defaultOption.text = "";
                                    defaultOption.value = "";
                                    select.add(defaultOption);
                                }
                                select.add(option.cloneNode(true));
                            }
                        });
                    });

                    if (response.d.length === 1) {
                        typeIds.forEach(id => {
                            const select = document.getElementById(id);
                            if (select) select.selectedIndex = 0;
                        });
                    }

                    const bindPromises = typeIds.map((id, idx) => {
                        const val = document.getElementById(id)?.value || "";
                        return bindFunctions[idx](val);
                    });

                    Promise.all(bindPromises).then(resolve).catch(reject);
                } else {
                    const bindPromises = typeIds.map((id, idx) => {
                        const val = document.getElementById(id)?.value || "";
                        return bindFunctions[idx](val);
                    });
                    Promise.all(bindPromises).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindFabricColour(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolour = document.getElementById("fabriccolour");
        fabriccolour.innerHTML = "";

        if (!fabricType) {
            resolve();
            return;
        }

        const listData = { type: "FabricColour", fabrictype: fabricType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    fabriccolour.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolour.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolour.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindFabricColourB(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolourb = document.getElementById("fabriccolourb");
        fabriccolourb.innerHTML = "";

        if (!fabricType) {
            resolve();
            return;
        }

        const listData = { type: "FabricColour", fabrictype: fabricType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    fabriccolourb.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolourb.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolourb.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolourb.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTrackType(heading) {
    return new Promise((resolve, reject) => {
        const tracktype = document.getElementById("tracktype");
        tracktype.innerHTML = "";

        if (!heading) {
            const selectedValue = tracktype.value || "";
            Promise.all([
                bindTrackColour(selectedValue)
            ]).then(resolve).catch(reject);
        }

        const listData = { type: "CurtainTrackType", customtype: heading, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    tracktype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        tracktype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        tracktype.add(option);
                    });

                    if (response.d.length === 1) {
                        tracktype.selectedIndex = 0;
                    }

                    const selectedValue = tracktype.value || "";
                    Promise.all([
                        bindTrackColour(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tracktype.value || "";
                    Promise.all([
                        bindTrackColour(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTrackTypeB(heading) {
    return new Promise((resolve, reject) => {
        const tracktypeb = document.getElementById("tracktypeb");
        tracktypeb.innerHTML = "";

        if (!heading) {
            const selectedValue = tracktypeb.value || "";
            Promise.all([
                bindTrackColourB(selectedValue)
            ]).then(resolve).catch(reject);
        }

        const listData = { type: "CurtainTrackType", customtype: heading, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    tracktypeb.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        tracktypeb.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        tracktypeb.add(option);
                    });

                    if (response.d.length === 1) {
                        tracktypeb.selectedIndex = 0;
                    }

                    const selectedValue = tracktypeb.value || "";
                    Promise.all([
                        bindTrackColourB(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tracktypeb.value || "";
                    Promise.all([
                        bindTrackColourB(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTrackColour(trackType) {
    return new Promise((resolve, reject) => {
        const trackcolour = document.getElementById("trackcolour");

        if (!trackType) {
            trackcolour.innerHTML = "";
            resolve();
            return;
        }

        const listData = { type: "CurtainTrackColour", customtype: trackType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    trackcolour.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        trackcolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        trackcolour.add(option);
                    });

                    if (response.d.length === 1) {
                        trackcolour.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTrackColourB(trackType) {
    return new Promise((resolve, reject) => {
        const trackcolourb = document.getElementById("trackcolourb");

        if (!trackType) {
            trackcolourb.innerHTML = "";
            resolve();
            return;
        }

        const listData = { type: "CurtainTrackColour", customtype: trackType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    trackcolourb.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        trackcolourb.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        trackcolourb.add(option);
                    });

                    if (response.d.length === 1) {
                        trackcolourb.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindComponentForm(blindType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divfirst", "divfirstend",
            "divsecond", "divsecondend",
            "divmouting",
            "divheading", "divheadingb",
            "divfabric", "divfabricb",
            "divtrack", "divtrackb",
            "divstackposition", "divstackpositionb",
            "divwidth", "divwidthb",
            "divdrop", "divdropb",
            "divcontrolcolour", "divcontrolcolourb",
            "divcontrollength", "divcontrollengthb",
            "divreturnlength", "divbottomhem", "divtieback",
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);
        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!colourType) return resolve();

        toggleDisplay(detail, true);

        getBlindName(blindType).then(blindName => {
            let divShow = [];

            if (blindName === "Single Curtain & Track") {
                divShow.push(
                    "divmouting", "divheading", "divfabric", "divtrack", "divstackposition", "divwidth", "divdrop", "divreturnlength", "divbottomhem", "divtieback",
                );
            } else if (blindName === "Double Curtain & Track") {
                divShow.push(
                    "divfirst", "divfirstend",
                    "divsecond", "divsecondend",
                    "divmouting",
                    "divheading", "divheadingb",
                    "divfabric", "divfabricb",
                    "divtrack", "divtrackb",
                    "divstackposition", "divstackpositionb",
                    "divwidth", "divwidthb",
                    "divdrop", "divdropb",
                    "divreturnlength", "divbottomhem", "divtieback",
                );
            } else if (blindName === "Curtain Only") {
                divShow.push(
                    "divmouting", "divheading", "divfabric", "divwidth", "divdrop", "divstackposition", "divreturnlength", "divbottomhem", "divtieback"
                );
            } else if (blindName === "Track Only") {
                divShow.push(
                    "divtrack", "divstackposition", "divwidth"
                );
            } else if (blindName === "Fabric Only") {
                divShow.push(
                    "divfabric", "divwidth", "divdrop"
                );
            }

            divShow.forEach(id => toggleDisplay(document.getElementById(id), true));

            if (typeof priceAccess !== "undefined" && priceAccess) {
                toggleDisplay(markup, true);
            }

            resolve();
        }).catch(error => {
            reject(error);
        });
    });
}

function visibleControlColourLength(number, trackDraw) {
    return new Promise((resolve, reject) => {
        let controlColour = null;
        let controlLength = null;

        if (number === 1) {
            controlColour = document.getElementById("divcontrolcolour");
            controlLength = document.getElementById("divcontrollength");
        } else if (number === 2) {
            controlColour = document.getElementById("divcontrolcolourb");
            controlLength = document.getElementById("divcontrollengthb");
        }

        if (!controlColour || !controlLength) {
            return resolve();
        }

        controlColour.style.display = "none";
        controlLength.style.display = "none";

        if (trackDraw === "Flick Stick") {
            controlColour.style.display = "";
            controlLength.style.display = "";
        }
        resolve();
    });
}

function otomatisWidth(blindType, blindNumber, width) {
    return new Promise((resolve, reject) => {
        if (!blindType || !blindNumber) {
            return resolve();
        }

        getBlindName(blindType).then(blindName => {
            if (blindName === "Double Curtain & Track") {
                if (blindNumber === 1) {
                    document.getElementById("widthb").value = width;
                } else if (blindNumber === 2) {
                    document.getElementById("width").value = width;
                }
            }
        }).catch(error => {
            reject(error);
        });
    });
}

function otomatisDrop(blindType, blindNumber, drop) {
    return new Promise((resolve, reject) => {
        if (!blindType || !blindNumber) {
            return resolve();
        }

        getBlindName(blindType).then(blindName => {
            if (blindName === "Double Curtain & Track") {
                if (blindNumber === 1) {
                    document.getElementById("dropb").value = drop;
                } else if (blindNumber === 2) {
                    document.getElementById("drop").value = drop;
                }
            }
        }).catch(error => {
            reject(error);
        });
    });
}

function toggleButtonState(disabled, text) {
    $("#submit")
        .prop("disabled", disabled)
        .css("pointer-events", disabled ? "none" : "auto")
        .text(text);

    $("#cancel").prop("disabled", disabled).css("pointer-events", disabled ? "none" : "auto");
}

function startCountdown(seconds) {
    let countdown = seconds;
    const button = $("#vieworder");

    function updateButton() {
        button.text(`View Order (${countdown}s)`);
        countdown--;

        if (countdown >= 0) {
            setTimeout(updateButton, 1000);
        } else {
            window.location.href = `/order/detail?orderid=${headerId}`;
        }
    }
    updateButton();
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) {
        isEditItem = false;
    }
    if (isCopyItem === undefined) {
        isCopyItem = false;
    }

    document.getElementById("submit").style.display = status ? "none" : "";

    const inputs = [
        "blindtype", "colourtype", "qty", "room", "mounting",
        "heading", "fabrictype", "fabriccolour", "tracktype", "trackcolour", "trackdraw", "stackposition", "width", "drop", "controlcolour", "controllength",
        "headingb", "fabrictypeb", "fabriccolourb", "tracktypeb", "trackcolourb", "trackdrawb", "stackpositionb", "widthb", "dropb", "controlcolourb", "controllengthb",
        "returnlengthvalue", "returnlengthvalueb", "bottomhem", "tieback",
        "notes", "markup"
    ];

    inputs.forEach(id => {
        const inputElement = document.getElementById(id);
        if (inputElement) {
            if (isCopyItem) {
                inputElement.disabled = (id === "blindtype");
            } else if (isEditItem && (id === "qty" || id === "blindtype")) {
                inputElement.disabled = true;
            } else {
                inputElement.disabled = status;
            }
        }
    });
}

function fillSelect(selector, list, selected = null) {
    const el = document.querySelector(selector);
    el.innerHTML = "<option value=''></option>";
    list.forEach(item => {
        const opt = document.createElement("option");
        opt.value = item.Value;
        opt.textContent = item.Text;
        if (selected != null && selected == item.Value) opt.selected = true;
        el.appendChild(opt);
    });
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        heading: "Heading",
        fabrictype: "FabricId",
        fabriccolour: "FabricColourId",
        tracktype: "TrackType",
        trackcolour: "TrackColour",
        trackdraw: "TrackDraw",
        stackposition: "StackPosition",
        controlcolour: "ControlColour",
        controllength: "ControlLengthValue",
        width: "Width",
        drop: "Drop",
        headingb: "HeadingB",
        fabrictypeb: "FabricIdB",
        fabriccolourb: "FabricColourIdB",
        tracktypeb: "TrackTypeB",
        trackcolourb: "TrackColourB",
        trackdrawb: "TrackDrawB",
        stackpositionb: "StackPositionB",
        controlcolourb: "ControlColourB",
        controllengthb: "ControlLengthValueB",
        widthb: "WidthB",
        dropb: "DropB",
        returnlengthvalue: "ReturnLengthValue",
        returnlengthvalueb: "ReturnLengthValueB",
        bottomhem: "BottomHem",
        tieback: "Supply",
        notes: "Notes",
        markup: "MarkUp"
    };

    Object.keys(mapping).forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;

        let value = itemData[mapping[id]];
        if (id === "markup" && value === 0) value = "";
        el.value = value || "";
    });

    if (itemAction === "copy") {
        const resetFields = ["room", "notes"];
        resetFields.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = "";
        });
    }
}

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "colourtype", "qty", "room", "mounting",
        "heading", "fabrictype", "fabriccolour", "tracktype", "trackcolour", "trackdraw", "stackposition", "width", "drop", "controlcolour", "controllength",
        "headingb", "fabrictypeb", "fabriccolourb", "tracktypeb", "trackcolourb", "trackdrawb", "stackpositionb", "widthb", "dropb", "controlcolourb", "controllengthb",
        "returnlengthvalue", "returnlengthvalueb", "bottomhem", "tieback",
        "notes", "markup"
    ];

    const formData = {
        headerid: headerId,
        orderid: orderId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId,
        rolename: roleAccess,
        customerid: customerId,
        companyid: companyId,
        companydetailid: companyDetailId
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/CurtainProcess",
        data: JSON.stringify({ data: formData }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d.trim();
            if (result === "Success") {
                setTimeout(() => {
                    $('#modalSuccess').modal('show');
                    startCountdown(3);
                }, 500);
            } else {
                isError(result);
                toggleButtonState(false, "Submit");
            }
        },
        error: function () {
            toggleButtonState(false, "Submit");
        }
    });
}

async function iniCurtain() {
    const urlParams = new URLSearchParams(window.location.search);
    const sessionId = urlParams.get("boos");

    if (!sessionId) return redirectOrder();

    const response = await fetch("Method.aspx/StringData", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify({ type: "OrderContext", dataId: sessionId })
    });

    const result = await response.json();
    if (!result?.d) return redirectOrder();

    const params = new URLSearchParams(result.d);

    itemAction = params.get("do");
    headerId = params.get("orderid");
    itemId = params.get("itemid");
    designId = params.get("dtype");
    loginId = params.get("uid");

    if (!headerId) return redirectOrder();

    updateLinkDetail(headerId);

    if (!itemAction || !designId || !loginId || designId !== designIdOri) {
        return window.location.href = `/order/detail?orderid=${headerId}`;
    }

    await Promise.all([
        getOrderHeader(headerId),
        getDesignName(designId),
        getFormAction(itemAction),
        getCompanyOrder(headerId),
        getCompanyDetailOrder(headerId),
        getRoleAccess(loginId),
        getPriceAccess(loginId)
    ]);

    if (itemAction === "create") {
        bindComponentForm("", "");
        controlForm(false);
        bindBlindType(designId);
        bindFabricType(designId);
        bindTrackType("");
        loader(itemAction);
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        await bindItemOrder(itemId, companyDetailId, itemAction);
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
    }
}

async function bindItemOrder(itemId, companyDetailId, action) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/CurtainDetail",
            data: JSON.stringify({ itemId, companyDetailId, action }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabrictypeb", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);
        fillSelect("#fabriccolourb", data.FabricColoursB);
        fillSelect("#tracktype", data.TrackTypes);
        fillSelect("#tracktypeb", data.TrackTypesB);
        fillSelect("#trackcolour", data.TrackColours);
        fillSelect("#trackcolourb", data.TrackColoursB);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        setFormValues(data.ItemData);

        bindComponentForm(data.ItemData.BlindType, data.ItemData.ProductId);
        visibleControlColourLength(1, data.ItemData.TrackDraw);
        visibleControlColourLength(2, data.ItemData.TrackDrawB);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function showInfo(type) {
    let info;

    if (type === "TieBack") {
        let img = "https://bigblinds.ordersblindonline.com/assets/images/products/tieback.jpg";
        //let img = "https://ordersblindonline.com/assets/images/products/tieback.jpg";

        info = "<b>Tie Back Information</b>";
        info += "<br /><br />";
        info += `<img src="${img}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
        info += "<br /><br />";
    }
    document.getElementById("spanInfo").innerHTML = info;
}

function showGallery(type) {
    let info;

    if (type === "Heading") {
        let urlImage = "https://bigblinds.ordersblindonline.com/Assets/images/products/curtain/heading.jpg";
        //let urlImage = "https://ordersblindonline.com/Assets/images/products/curtain/heading.jpg";
        info = `<img src="${urlImage}" style="max-width:100%;height:auto;">`;
    } else if (type === "Style") {
        let urlImage = "https://bigblinds.ordersblindonline.com/Assets/images/products/curtain/styletrack.jpg";
        //let urlImage = "https://ordersblindonline.com/Assets/images/products/curtain/styletrack.jpg";
        info = `<img src="${urlImage}" style="max-width:100%;height:auto;">`;
    } else if (type === "Commercial") {
        let urlImage = "https://bigblinds.ordersblindonline.com/Assets/images/products/curtain/commercialtrack.jpg";
        //let urlImage = "https://ordersblindonline.com/Assets/images/products/curtain/commercialtrack.jpg";
        info = `<img src="${urlImage}" style="max-width:100%;height:auto;">`;
    }
    document.getElementById("spanInfoGallery").innerHTML = info;
}

function redirectOrder() {
    window.location.replace("/order");
}

function updateLinkDetail(myId) {
    const link = document.getElementById("orderDetail");
    if (!link || !headerId) return;

    link.href = `/order/detail?orderid=${myId}`;
}

document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalError").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalInfo").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalGallery").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});