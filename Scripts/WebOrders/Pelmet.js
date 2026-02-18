let designIdOri = "7";
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

initPelmet();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindTubeType($(this).val());
    bindMounting($(this).val());
});

$("#tubetype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindColourType(blindtype, $(this).val());
});

$("#colourtype").on("change", function () {
    bindComponentForm($(this).val());
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#layoutcode").on("change", function () {
    visibleWidth($(this).val());
});

$("#returnposition").on("change", function () {
    visiblReturnLength($(this).val());
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

        pageAction.innerText = actionMap[itemAction];
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

function bindBlindType(designType) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designType) {
            const selectedValue = blindtype.value || "";
            Promise.all([
                bindMounting(selectedValue),
                bindTubeType(selectedValue)
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
                        bindMounting(selectedValue),
                        bindTubeType(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindTubeType(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindTubeType(blindType) {
    return new Promise((resolve, reject) => {
        const tubetype = document.getElementById("tubetype");
        tubetype.innerHTML = "";

        if (!blindType) {
            const selectedValue = tubetype.value || "";
            Promise.all([
                bindColourType(blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "TubeType", companydetailid: companyDetailId, blindtype: blindType, action: itemAction };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    tubetype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        tubetype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        tubetype.add(option);
                    });

                    if (response.d.length === 1) {
                        tubetype.selectedIndex = 0;
                    }

                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindColourType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindColourType(blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType, tubeType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType || !tubeType) {
            const selectedValue = colourtype.value || "";
            Promise.all([
                bindComponentForm(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", companydetailid: companyDetailId, blindtype: blindType, tubetype: tubeType, controltype: "0", action: itemAction };

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
                        bindComponentForm(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        bindComponentForm(selectedValue)
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
        const fabrictype = document.getElementById("fabrictype");
        fabrictype.innerHTML = "";

        if (!designType) {
            const selectedValue = fabrictype.value || "";
            Promise.resolve(
                bindFabricColour(selectedValue)
            ).then(resolve).catch(reject);
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
                    fabrictype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabrictype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabrictype.add(option);
                    });

                    if (response.d.length === 1) {
                        fabrictype.selectedIndex = 0;
                    }

                    const selectedValue = fabrictype.value || "";
                    Promise.resolve(
                        bindFabricColour(selectedValue)
                    ).then(resolve).catch(reject);
                } else {
                    const selectedValue = fabrictype.value || "";
                    Promise.resolve(
                        bindFabricColour(selectedValue)
                    ).then(resolve).catch(reject);
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
        "blindtype", "tubetype", "colourtype",
        "qty", "room", "mounting",
        "fabrictype", "fabriccolour", "batten",
        "layoutcode", "width", "widthb", "widthc",
        "returnposition", "returnlengthvalue", "returnlengthvalueb", "supply",
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

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        tubetype: "TubeType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        width: "Width",
        widthb: "WidthB",
        widthc: "WidthC",
        fabrictype: "FabricId",
        fabriccolour: "FabricColourId",
        layoutcode: "LayoutCode",
        batten: "Batten",
        returnposition: "ReturnPosition",
        returnlengthvalue: "ReturnLengthValue",
        returnlengthvalueb: "ReturnLengthValueB",
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

function bindComponentForm(colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const widthb = document.getElementById("divwidthb");
        const widthc = document.getElementById("divwidthc");
        const returnlength = document.getElementById("divreturnlength");
        const returnlengthb = document.getElementById("divreturnlengthb");
        const markup = document.getElementById("divmarkup");

        function toggleDisplay(element, show) {
            if (element) element.style.display = show ? "" : "none";
        }

        toggleDisplay(detail, false);
        toggleDisplay(widthb, false);
        toggleDisplay(widthc, false);
        toggleDisplay(returnlength, false);
        toggleDisplay(returnlengthb, false);
        toggleDisplay(markup, false);

        if (!colourType) return resolve();

        toggleDisplay(detail, true);

        if (typeof priceAccess !== "undefined" && priceAccess) {
            toggleDisplay(markup, true);
        }

        resolve();
    });
}

function visibleWidth(layoutCode) {
    return new Promise((resolve) => {
        const widthb = document.getElementById("divwidthb");
        const widthc = document.getElementById("divwidthc");

        function toggleDisplay(el, show) {
            if (el) el.style.display = show ? "" : "none";
        }

        toggleDisplay(widthb, false);
        toggleDisplay(widthc, false);

        if (layoutCode === "B" || layoutCode === "C") {
            toggleDisplay(widthb, true);
        } else if (layoutCode === "D") {
            toggleDisplay(widthb, true);
            toggleDisplay(widthc, true);
        }

        resolve();
    });
}

function visiblReturnLength(retrunPosition) {
    return new Promise((resolve, reject) => {
        const returnlength = document.getElementById("divreturnlength");
        const returnlengthb = document.getElementById("divreturnlengthb");

        returnlength.style.display = "none";
        returnlengthb.style.display = "none";

        if (retrunPosition === "Left") {
            returnlength.style.display = "";
        } else if (retrunPosition === "Right") {
            returnlengthb.style.display = "";
        } else if (retrunPosition === "Both Sides") {
            returnlength.style.display = "";
            returnlengthb.style.display = "";
        }
        resolve();
    });
}

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "tubetype", "colourtype",
        "qty", "room", "mounting",
        "fabrictype", "fabriccolour", "batten",
        "layoutcode", "width", "widthb", "widthc",
        "returnposition", "returnlengthvalue", "returnlengthvalueb", "supply",
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
        url: "Method.aspx/PelmetProcess",
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

async function bindItemOrder(itemId, companyDetailId, action) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/PelmetDetail",
            data: JSON.stringify({ itemId, companyDetailId, action }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#tubetype", data.TubeTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        bindComponentForm(data.ItemData.ProductId);
        visibleWidth(data.ItemData.LayoutCode);
        visiblReturnLength(data.ItemData.ReturnPosition);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}


async function initPelmet() {
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
        bindComponentForm("");
        controlForm(false);
        bindBlindType(designId);
        bindFabricType(designId);
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

function redirectOrder() {
    window.location.replace("/order");
}

function updateLinkDetail(myId) {
    const link = document.getElementById("orderDetail");
    if (!link || !headerId) return;

    link.href = `/order/detail?orderid=${myId}`;
}

function showInfo(type) {
    let info;

    if (type === "Batten Colour") {
        let urlImage = "https://ordersblindonline.com/Assets/images/products/pelmet/timbercolour.JPG";

        info = "Batten Colour";
        info = `<img src="${urlImage}" style="max-width:100%;height:auto;">`;
    }
    document.getElementById("spanInfo").innerHTML = info;
}

function showGallery(type) {
    let info;

    if (type === "Layout") {
        let urlImage = "https://ordersblindonline.com/Assets/images/products/pelmet/layout.jpg";

        info = `<img src="${urlImage}" style="max-width:100%;height:auto;">`;
    }
    document.getElementById("spanInfoGallery").innerHTML = info;
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