let designIdOri = "10";
let itemAction;
let headerId;
let orderId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

initVenetian();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindSubType($(this).val());
    bindMounting($(this).val());
    bindColourType($(this).val());
    bindValanceType($(this).val());
    bindValancePosition($(this).val());
});

$("#subtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;

    const width = parseFloat(document.getElementById("width").value) || 0;
    const drop = parseFloat(document.getElementById("drop").value) || 0;

    bindControlPosition($(this).val(), width);
    bindTilterPosition($(this).val(), width);

    bindComponentForm(blindtype, $(this).val());

    otomatisDrop($(this).val(), 1, drop);

    document.getElementById("controllength").value = "";
    document.getElementById("controllengthb").value = "";
    document.getElementById("valancesize").value = "";
    document.getElementById("returnlength").value = "";
});

$("#width").on("input", function () {
    const subtype = document.getElementById("subtype").value;
    const width = parseFloat(document.getElementById("width").value) || 0;

    bindControlPosition(subtype, width);
    bindTilterPosition(subtype, width);
});

$("#width").on("blur", function () {
    const subtype = document.getElementById("subtype").value;
    const width = parseFloat(document.getElementById("width").value) || 0;

    if (subtype === "Single" && roleAccess === "Customer" && width < 300) {
        isError("PLEASE NOTE THAT YOUR ORDER WIDTH IS NOT COVERED UNDER OUR WARRANTY.");
    }
});

$("#drop").on("input", function () {
    const subtype = document.getElementById("subtype").value;
    otomatisDrop(subtype, 1, $(this).val());
});

$("#dropb").on("input", function () {
    const subtype = document.getElementById("subtype").value;
    otomatisDrop(subtype, 2, $(this).val());
});

$("#controllength").on("change", function () {
    visibleCustom("CordLength", $(this).val(), "1");
});

$("#controllengthb").on("change", function () {
    visibleCustom("CordLength", $(this).val(), "2");
});

$("#valancesize").on("change", function () {
    visibleCustom("ValanceSize", $(this).val(), "");
});

$("#returnlength").on("change", function () {
    visibleCustom("ValanceLength", $(this).val(), "");
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
                document.getElementById("orderid").innerText = d.OrderId || "-";
                document.getElementById("ordernumber").innerText = d.OrderNumber || "-";
                document.getElementById("ordername").innerText = d.OrderName || "-";
                resolve(d);
            },
            error: reject
        });
    });
}

function getCompanyOrder(headerId) {
    return new Promise((resolve, reject) => {
        company = "";

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
                company = response.d.trim();
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
        companyDetail = "";

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
                companyDetail = response.d.trim();
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

        pageAction.innerText = actionMap[itemAction];
        resolve();
    });
}

function bindBlindType(designType) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designType) {
            const selectedValue = blindtype.value || "";
            Promise.all([
                bindMounting(selectedValue),
                bindColourType(selectedValue),
                bindValanceType(selectedValue),
                bindValancePosition(selectedValue)
            ]).then(resolve).catch(reject);

            return;
        }

        const listData = { type: "BlindType", companydetail: companyDetail, designtype: designType };
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
                        bindMounting(selectedValue),
                        bindColourType(selectedValue),
                        bindValanceType(selectedValue),
                        bindValancePosition(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindColourType(selectedValue),
                        bindValanceType(selectedValue),
                        bindValancePosition(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindtype) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindtype) {
            resolve();
            return;
        }

        const listData = { type: "ColourType", blindtype: blindtype, companydetail: companyDetail, tubetype: "0", controltype: "0" };
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
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindSubType(blindType) {
    return new Promise((resolve, reject) => {
        const subtype = document.getElementById("subtype");
        subtype.innerHTML = "";

        if (!blindType) {
            const selectedValue = subtype.value || "";
            bindComponentForm(blindType, selectedValue);
            resolve();
            return;
        }

        const listData = { type: "SubType", blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    subtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        subtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        subtype.add(option);
                    });

                    if (response.d.length === 1) {
                        subtype.selectedIndex = 0;
                    }

                    const selectedValue = subtype.value || "";
                    Promise.all([
                        bindComponentForm(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = subtype.value || "";
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

        const listData = { type: "Mounting", blindtype: blindType };

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

function bindControlPosition(subType, width) {
    return new Promise((resolve) => {
        const controlposition = document.getElementById("controlposition");
        controlposition.innerHTML = "";

        if (!subType || width === 0) {
            resolve();
            return;
        }

        let options = [
            { value: "", text: "" },
            { value: "Left", text: "Left" },
            { value: "Right", text: "Right" }
        ];

        const widthRules = [
            {
                subType: "Single",
                min: 200,
                max: 250,
                options: [
                    { value: "", text: "" },
                    { value: "N/A", text: "N/A" }
                ]
            }
        ];

        const rule = widthRules.find(r =>
            r.subType === subType &&
            width >= r.min &&
            width <= r.max
        );

        if (rule) {
            options = rule.options;
        }

        options.forEach(opt => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            controlposition.appendChild(optionElement);
        });

        resolve();
    });
}

function bindTilterPosition(subType, width) {
    return new Promise((resolve) => {
        const tilterposition = document.getElementById("tilterposition");
        tilterposition.innerHTML = "";

        if (!subType || width === 0) {
            resolve();
            return;
        }

        let options = [
            { value: "", text: "" },
            { value: "Left", text: "Left" },
            { value: "Right", text: "Right" }
        ];

        const widthRules = [
            {
                subType: "Single",
                min: 200,
                max: 250,
                options: [
                    { value: "", text: "" },
                    { value: "Centre", text: "Centre" }
                ]
            }
        ];

        const rule = widthRules.find(r =>
            r.subType === subType &&
            width >= r.min &&
            width <= r.max
        );

        if (rule) {
            options = rule.options;
        }

        options.forEach(opt => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            tilterposition.appendChild(optionElement);
        });

        resolve();
    });
}

function bindValanceType(blindType) {
    return new Promise((resolve, reject) => {
        const valancetype = document.getElementById("valancetype");
        valancetype.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        const listData = { type: "ValanceType", blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    valancetype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        valancetype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        valancetype.add(option);
                    });

                    if (response.d.length === 1) {
                        valancetype.selectedIndex = 0;
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

function bindValancePosition(blindType) {
    return new Promise((resolve, reject) => {
        const returnposition = document.getElementById("returnposition");
        returnposition.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        const listData = { type: "ValancePosition", blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    returnposition.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        returnposition.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        returnposition.add(option);
                    });

                    if (response.d.length === 1) {
                        returnposition.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });

    return new Promise((resolve, reject) => {
        if (!blindType) {
            resolve();
            return;
        }

        const returnposition = document.getElementById("");
        returnposition.innerHTML = "";

        if (!blindType) return;

        getBlindName(blindType).then(blindName => {
            let options = [
                { value: "", text: "" },
                { value: "", text: "Left" },
                { value: "", text: "Right" },
                { value: "", text: "Both Sides" }
            ];

            if (blindName === "Econo 50mm (Cordless)") {
                options = options = [
                    { value: "", text: "" },
                    { value: "Left", text: "Left" },
                    { value: "Right", text: "Right" }
                ];
            }

            const fragment = document.createDocumentFragment();
            options.forEach(opt => {
                const optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                fragment.appendChild(optionElement);
            });
            returnposition.appendChild(fragment);

            resolve();
        }).catch(error => {
            reject(error);
        });
    });
}

function bindComponentForm(blindType, subType) {
    return new Promise((resolve, reject) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divfirstblind", "divfirstend", "divsecondblind", "divsecondend",
            "divtassel",
            "divcontrolposition", "divtilterposition",
            "divcordlength", "divcordlengthb",
            "divcordlengthvalue", "divcordlengthvalueb",
            "divsize", "divsizeb",
            "divwandlength",
            "divvalancesection",
            "divvalancesizevalue", "divvalancelengthvalue"
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);

        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!blindType || !subType) {
            return resolve();
        }

        toggleDisplay(detail, true);

        getBlindName(blindType).then(blindName => {
            let divShow = [];

            if (blindName === "Basswood 50mm" || blindName === "Basswood 63mm" || blindName === "Econo 50mm" || blindName === "Econo 63mm" || blindName === "Ultraslat 50mm" || blindName === "Ultraslat 63mm") {
                divShow.push("divtassel", "divvalancesection");

                if (subType === "Single") {
                    divShow.push("divcontrolposition", "divtilterposition", "divsize", "divcordlength");
                }
                else if (subType === "2 on 1 Left-Left" || subType === "2 on 1 Right-Right") {
                    divShow.push("divfirstblind", "divfirstend", "divsecondblind", "divsecondend", "divsize", "divsizeb", "divcordlength", "divcordlengthb");
                }
                else if (subType === "2 on 1 Left-Right") {
                    divShow.push("divfirstblind", "divfirstend", "divsecondblind", "divsecondend", "divsize", "divsizeb", "divcordlength", "divcordlengthb");
                }
            } else if (blindName === "Econo 50mm (Cordless)") {
                divShow.push("divsize", "divtilterposition", "divwandlength", "divvalancesection",);
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

function visibleCustom(type, text, number) {
    return new Promise((resolve) => {
        if (!type) {
            resolve();
            return;
        }

        let thisDiv;

        if (type === "ValanceSize") {
            thisDiv = document.getElementById("divvalancesizevalue");
        } else if (type === "ValanceLength") {
            thisDiv = document.getElementById("divvalancelengthvalue");
        } else if (type === "CordLength") {
            if (number === "1") {
                thisDiv = document.getElementById("divcordlengthvalue");
            } else if (number === "2") {
                thisDiv = document.getElementById("divcordlengthvalueb");
            }
        }

        if (!thisDiv) {
            return resolve();
        }
        thisDiv.style.display = (text === "Custom") ? "" : "none";

        resolve();
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
        "blindtype", "colourtype", "qty", "room", "mounting", "subtype",
        "controlposition", "tilterposition",
        "width", "drop", "widthb", "dropb",
        "controllength", "controllengthvalue", "controllengthb", "controllengthvalueb",
        "valancetype", "valancesize", "valancesizevalue", "returnposition", "returnlength", "returnlengthvalue", "wandlengthvalue",
        "tassel", "supply", "notes", "markup"
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

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        subtype: "SubType",
        width: "Width",
        widthb: "WidthB",
        drop: "Drop",
        dropb: "DropB",
        controlposition: "ControlPosition",
        tilterposition: "TilterPosition",
        controllength: "ControlLength",
        controllengthvalue: "ControlLengthValue",
        controllengthb: "ControlLengthB",
        controllengthvalueb: "ControlLengthValueB",
        wandlengthvalue: "WandLengthValue",
        valancetype: "ValanceType",
        valancesize: "ValanceSize",
        valancesizevalue: "ValanceSizeValue",
        returnposition: "ReturnPosition",
        returnlength: "ReturnLength",
        returnlengthvalue: "ReturnLengthValue",
        tassel: "Tassel",
        supply: "Supply",
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

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "colourtype", "qty", "room", "mounting", "subtype",
        "controlposition", "tilterposition",
        "width", "drop", "widthb", "dropb",
        "controllength", "controllengthvalue", "controllengthb", "controllengthvalueb",
        "valancetype", "valancesize", "valancesizevalue", "returnposition", "returnlength", "returnlengthvalue", "wandlengthvalue",
        "tassel", "supply", "notes", "markup"
    ];

    const formData = {
        headerid: headerId,
        orderid: orderId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId,
        rolename: roleAccess,
        companyid: company,
        companydetailid: companyDetail
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/VenetianProcess",
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

async function initVenetian() {
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
        loader(itemAction);
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        await bindItemOrder(itemId, companyDetail);
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
    }
}

async function bindItemOrder(itemId, companyDetailId) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/VenetianDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#subtype", data.SubTypes);
        fillSelect("#valancetype", data.ValanceTypes);
        fillSelect("#returnposition", data.ValancePositions);

        let manual = [
            bindControlPosition(data.ItemData.SubType, data.ItemData.Width),
            bindTilterPosition(data.ItemData.SubType, data.ItemData.Width)
        ];
        await Promise.all(manual);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        bindComponentForm(data.ItemData.BlindType, data.ItemData.SubType);
        visibleCustom("CordLength", data.ItemData.ControlLength, "1");
        visibleCustom("CordLength", data.ItemData.ControlLengthB, "2");
        visibleCustom("ValanceSize", data.ItemData.ValanceSize, "");
        visibleCustom("ValanceLength", data.ItemData.ReturnLength, "");
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function otomatisDrop(subType, number, drop) {
    return new Promise((resolve) => {
        if (!subType || !number) return resolve();

        if (subType === "2 on 1 Left-Left" || subType === "2 on 1 Left-Right" || subType === "2 on 1 Right-Right") {
            if (number === 1) {
                document.getElementById("dropb").value = drop;
            } else if (number === 2) {
                document.getElementById("drop").value = drop;
            }
        }
        resolve();
    });
}

function showInfo(type) {
    let info;

    if (type === "Cord Length") {
        info = "<b>Cord Length Information</b>";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "Our standard pull cord length is 2/3 from your drop.";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "Minimum custom cord length is 450mm.";
    } else if (type === "Valance Size") {
        info = "<b>Valance Size Information</b>";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "";
    } else if (type === "Return Length") {
        info = "<b>Valance Return Length Information</b>";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "";
    } else if (type === "Second Size") {
        info = "<b>Drop Information</b>";
    } else if (type === "First") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleft-1.png";

        const subType = document.getElementById("subtype").value;
        if (subType === "2 on 1 Left-Left") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleft-1.png";
        } else if (subType === "2 on 1 Right-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianright-1.png";
        } else if (subType === "2 on 1 Left-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleftright-1.png";
        }

        info = "<b>Layout Information</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
        info += "<br /><br />";
    } else if (type === "Second") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleft-1.png";

        const subType = document.getElementById("subtype").value;
        if (subType === "2 on 1 Left-Left") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleft-2.png";
        } else if (subType === "2 on 1 Right-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianright-2.png";
        } else if (subType === "2 on 1 Left-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/2on1venetianleftright-2.png";
        }

        info = "<b>Layout Information</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
        info += "<br /><br />";
    }
    document.getElementById("spanInfo").innerHTML = info;
}

function showGallery(type) {
    let info;

    if (type === "Basswood") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/basswoodgallery.jpg";

        info = "<b>Basswood Gallery</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
    } else if (type === "Econo") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/econogallery.jpg";

        info = "<b>Econo Gallery</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
    } else if (type === "Cordless") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/venetian/cordlessgallery.jpg";

        info = "<b>Econo Cordless Gallery</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
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

document.getElementById("modalLayout").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalGallery").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});