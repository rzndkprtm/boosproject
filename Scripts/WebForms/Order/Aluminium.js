let designIdOri = "1";
let itemAction;
let headerId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

initAluminium();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    const blindtype = $(this).val();
    bindMounting(blindtype);
    bindColourType(blindtype);
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindSubType(blindtype, $(this).val());
});

$("#subtype").on("change", function () {
    const colourtype = document.getElementById("colourtype").value;
    const drop = document.getElementById("drop").value;
    bindComponentForm(colourtype, $(this).val());

    otomatisDrop($(this).val(), "1", drop);

    document.getElementById("controllength").value = "";
    document.getElementById("controllengthb").value = "";
    document.getElementById("controllengthvalue").value = "";
    document.getElementById("controllengthvalueb").value = "";

    document.getElementById("wandlength").value = "";
    document.getElementById("wandlengthb").value = "";
    document.getElementById("wandlengthvalue").value = "";
    document.getElementById("wandlengthvalueb").value = "";
});

$("#drop").on("input", function () {
    const subtype = document.getElementById("subtype").value;
    otomatisDrop(subtype, "1", $(this).val());
});

$("#dropb").on("input", function () {
    const subtype = document.getElementById("subtype").value;
    otomatisDrop(subtype, "2", $(this).val());
});

$("#controllength").on("change", function () {
    visibleCustom("CordLength", $(this).val(), "1");
});

$("#controllengthb").on("change", function () {
    visibleCustom("CordLength", $(this).val(), "2");
});

$("#wandlength").on("change", function () {
    visibleCustom("WandLength", $(this).val(), "1");
});

$("#wandlengthb").on("change", function () {
    visibleCustom("WandLength", $(this).val(), "2");
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

function getDesignName(designId) {
    return new Promise((resolve, reject) => {
        const cardTitle = document.getElementById("cardtitle");
        cardTitle.textContent = "";

        if (!designId) {
            resolve();
            return;
        }

        const type = "DesignName";
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: designId }),
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
    if (!blindType) return Promise.resolve(null);;

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
            create: "Add Item",
            edit: "Edit Item",
            view: "View Item",
            copy: "Copy Item"
        };
        pageAction.innerText = actionMap[itemAction] || "";
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
                bindColourType(selectedValue)
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
                        bindColourType(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindColourType(selectedValue)
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
                bindSubType(blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", blindtype: blindType, companydetail: companyDetail, tubetype: "0", controltype: "0" };

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
                        bindSubType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        bindSubType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindSubType(blindType, colourType) {
    return new Promise((resolve, reject) => {
        const subtype = document.getElementById("subtype");
        subtype.innerHTML = "";

        if (!blindType || !colourType) {
            const selectedValue = subtype.value || "";
            bindComponentForm(colourType, selectedValue);
            resolve();
            return;
        }

        let options = [
            { value: "", text: "" },
            { value: "Single", text: "Single Aluminium" },
            { value: "2 on 1 Left-Left", text: "2 on 1 Aluminium (Left-Left)" },
            { value: "2 on 1 Right-Right", text: "2 on 1 Aluminium (Right-Right)" },
            { value: "2 on 1 Left-Right", text: "2 on 1 Aluminium (Left-Right)" },
        ];

        const fragment = document.createDocumentFragment();
        options.forEach(opt => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            fragment.appendChild(optionElement);
        });
        subtype.appendChild(fragment);

        if (subtype.value === "") {
            const selectedValue = subtype.value || "";
            bindComponentForm(colourType, selectedValue);
            resolve();
            return;
        }
        resolve();
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

function bindComponentForm(colourType, subType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divfirstblind", "divfirstend", "divsecondblind", "divsecondend",
            "divcontrolposition", "divtilterposition",
            "divwidthb", "divdropb",
            "divcordlength", "divwandlength",
            "divcordlengthb", "divwandlengthb",
            "divcordlengthvalue", "divwandlengthvalue",
            "divcordlengthvalueb", "divwandlengthvalueb"
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);
        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!colourType || !subType) return resolve();

        toggleDisplay(detail, true);

        const divShow = [];

        if (subType === "Single") {
            divShow.push("divcontrolposition", "divtilterposition", "divcordlength", "divwandlength");
        } else if (subType === "2 on 1 Left-Left") {
            divShow.push(
                "divfirstblind", "divfirstend", "divsecondblind", "divsecondend",
                "divcordlength", "divwandlength",
                "divwidthb", "divdropb", "divcordlengthb"
            );
        } else if (subType === "2 on 1 Right-Right") {
            divShow.push(
                "divfirstblind", "divfirstend", "divsecondblind", "divsecondend",
                "divcordlength",
                "divwidthb", "divdropb", "divcordlengthb", "divwandlengthb"
            );
        } else if (subType === "2 on 1 Left-Right") {
            divShow.push(
                "divfirstblind", "divfirstend", "divsecondblind", "divsecondend",
                "divcordlength", "divwandlength",
                "divwidthb", "divdropb", "divcordlengthb", "divwandlengthb"
            );
        }

        divShow.forEach(id => {
            const el = document.getElementById(id);
            toggleDisplay(el, true);
        });

        if (typeof priceAccess !== "undefined" && priceAccess) {
            toggleDisplay(markup, true);
        }

        resolve();
    });
}

function visibleCustom(type, text, number) {
    return new Promise((resolve, reject) => {
        let thisDiv;

        if (type === "CordLength") {
            if (number === "1") {
                thisDiv = document.getElementById("divcordlengthvalue");
            } else if (number === "2") {
                thisDiv = document.getElementById("divcordlengthvalueb");
            }
        } else if (type === "WandLength") {
            if (number === "1") {
                thisDiv = document.getElementById("divwandlengthvalue");
            } else if (number === "2") {
                thisDiv = document.getElementById("divwandlengthvalueb");
            }
        }

        if (thisDiv) {
            thisDiv.style.display = "none";
            if (text === "Custom") {
                thisDiv.style.display = "";
            }
            resolve();
        }
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
        "wandlength", "wandlengthvalue", "wandlengthb", "wandlengthvalueb",
        "supply", "notes", "markup"
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
        wandlength: "WandLength",
        wandlengthvalue: "WandLengthValue",
        wandlengthb: "WandLengthB",
        wandlengthvalueb: "WandLengthValueB",
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

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "colourtype", "qty", "room", "mounting", "subtype",
        "controlposition", "tilterposition",
        "width", "drop", "widthb", "dropb",
        "controllength", "wandlength",
        "controllengthb", "wandlengthb",
        "controllengthvalue", "wandlengthvalue",
        "controllengthvalueb", "wandlengthvalueb",
        "supply", "notes", "markup"
    ];

    const formData = {
        headerid: headerId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/AluminiumProcess",
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

function otomatisDrop(subType, number, drop) {
    return new Promise((resolve) => {
        if (!subType || !number) return resolve();

        if (subType === "2 on 1 Left-Left" || subType === "2 on 1 Left-Right" || subType === "2 on 1 Right-Right") {
            let elementId = null;
            if (number === "1") {
                elementId = "dropb";
            } else if (number === "2") {
                elementId = "drop";
            }

            if (elementId) {
                const el = document.getElementById(elementId);
                if (el) {
                    el.value = drop;
                }
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
    }
    else if (type === "Second Drop") {
        info = "<b>Drop Information</b>";
        info += "<br /><br />";
        info += "The second drop automatically matches and follows the first drop.";
        info += "<br />";
        info += "If the second drop is changed, the first drop will automatically adjust to match it.";
    } else if (type === "Wand Length") {
        info = "<b>Wand Length Information</b>";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "Our standard wand length is 2/3 from your drop.";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "Minimum custom wand length is 450mm.";
    } else if (type === "First") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleft-1.png";

        const subType = document.getElementById("subtype").value;
        if (subType === "2 on 1 Left-Left") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleft-1.png";
        } else if (subType === "2 on 1 Right-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumright-1.png";
        } else if (subType === "2 on 1 Left-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleftright-1.png";
        }

        info = "<b>Layout Information</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
        info += "<br /><br />";
    } else if (type === "Second") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleft-1.png";

        const subType = document.getElementById("subtype").value;
        if (subType === "2 on 1 Left-Left") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleft-2.png";
        } else if (subType === "2 on 1 Right-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumright-2.png";
        } else if (subType === "2 on 1 Left-Right") {
            urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/2on1aluminiumleftright-2.png";
        }

        info = "<b>Layout Information</b>";
        info += "<br /><br />";
        info += `<img src="${urlImage}" alt="Sub Type Image" style="max-width:100%;height:auto;">`;
        info += "<br /><br />";
    }
    document.getElementById("spanInfo").innerHTML = info;
}

async function initAluminium() {
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
        bindBlindType(designId);
        bindComponentForm("", "");
        controlForm(false);
        loader(itemAction);
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
        await bindItemOrder(itemId, companyDetail);
    }
}

async function bindItemOrder(itemId, companyDetailId) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/AluminiumDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#colourtype", data.ColourTypes);
        bindSubType(data.ItemData.BlindType, data.ItemData.ProductId);
        fillSelect("#mounting", data.Mountings);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        bindComponentForm(data.ItemData.ProductId, data.ItemData.SubType);
        visibleCustom("CordLength", data.ItemData.ControlLength, "1");
        visibleCustom("CordLength", data.ItemData.ControlLengthB, "2");
        visibleCustom("WandLength", data.ItemData.WandLength, "1");
        visibleCustom("WandLength", data.ItemData.WandLengthB, "2");
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function redirectOrder() {
    window.location.replace("/order");
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