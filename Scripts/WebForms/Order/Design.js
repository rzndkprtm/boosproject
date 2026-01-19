let designIdOri = "4";
let itemAction;
let headerId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

initDesign();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    const blindtype = $(this).val();
    bindControlType(blindtype);
    bindMounting(blindtype);
});

$("#controltype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindColourType(blindtype, $(this).val());
    bindChainColour(designId, $(this).val());

    document.getElementById("controllength").value = "";
});

$("#colourtype").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    bindComponentForm(controltype, $(this).val());
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#controllength").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    visibleCustom(controltype, $(this).val());
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
        pageAction.innerText = actionMap[itemAction];
        resolve();
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

function getControlName(controlType) {
    if (!controlType) return;

    const type = "ControlName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: controlType }),
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
                bindMounting(selectedValue),
                bindControlType(selectedValue)
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
                        bindControlType(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindControlType(selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindControlType(blindType) {
    return new Promise((resolve, reject) => {
        const controltype = document.getElementById("controltype");
        controltype.innerHTML = "";

        if (!blindType) {
            const selectedValue = controltype.value || "";
            Promise.all([
                bindColourType(blindType, selectedValue),
                bindChainColour(designId, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "ControlType", companydetail: companyDetail, blindtype: blindType, tubetype: "0" };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    controltype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        controltype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        controltype.add(option);
                    });

                    if (response.d.length === 1) {
                        controltype.selectedIndex = 0;
                    }

                    const selectedValue = controltype.value || "";
                    Promise.all([
                        bindColourType(blindType, selectedValue),
                        bindChainColour(designId, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = controltype.value || "";
                    Promise.all([
                        bindColourType(blindType, selectedValue),
                        bindChainColour(designId, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType, controlType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType || !controlType) {
            const selectedValue = colourtype.value || "";
            Promise.all([
                bindComponentForm(controlType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", companydetail: companyDetail, blindtype: blindType, tubetype: "0", controltype: controlType };

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
                        bindComponentForm(controlType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        bindComponentForm(controlType, selectedValue)
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

        const listData = { type: "FabricTypeByDesign", designtype: designType, companydetail: companyDetail };

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

        const listData = { type: "FabricColour", fabrictype: fabricType };

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

function bindChainColour(designType, controlType) {
    return new Promise((resolve, reject) => {
        const chaincolour = document.getElementById("chaincolour");
        chaincolour.innerHTML = "";

        if (!designType || !controlType) {
            resolve();
            return;
        }

        const listData = { type: "ControlColour", designtype: designType, controltype: controlType, companydetail: companyDetail };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chaincolour.innerHTML = "";
                    if (response.d.length > 1) {
                        var defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chaincolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        var option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chaincolour.add(option);
                    });
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindComponentForm(controlType, colourType) {
    return new Promise((resolve, reject) => {
        const detail = document.getElementById("divdetail");
        const controlposition = document.getElementById("divcontrolposition");
        const chaincolour = document.getElementById("divchaincolour");
        const wandcolour = document.getElementById("divwandcolour");
        const chainlength = document.getElementById("divcustomchainlength");
        const wandlength = document.getElementById("divcustomwandlength");
        const markup = document.getElementById("divmarkup");
        const controllength = document.getElementById("divcontrollength");

        function toggleDisplay(element, show) {
            if (element) element.style.display = show ? "" : "none";
        }

        toggleDisplay(detail, false);
        toggleDisplay(controlposition, false);
        toggleDisplay(chaincolour, false);
        toggleDisplay(wandcolour, false);
        toggleDisplay(chainlength, false);
        toggleDisplay(wandlength, false);
        toggleDisplay(markup, false);
        toggleDisplay(controllength, false);

        if (!colourType) return resolve();

        toggleDisplay(detail, true);

        getControlName(controlType).then(controlName => {
            toggleDisplay(controllength, true);

            if (controlName === "Chain") {
                toggleDisplay(controlposition, true);
                toggleDisplay(chaincolour, true);
                $("#controllengthtitle").text("Chain Length");
            } else if (controlName === "Wand") {
                toggleDisplay(wandcolour, true);
                $("#controllengthtitle").text("Wand Length");
            }

            resolve();
        }).catch(error => {
            resolve();
        });

        if (typeof priceAccess !== "undefined" && priceAccess) {
            toggleDisplay(markup, true);
        }
    });
}

function visibleCustom(controlType, text) {
    return new Promise((resolve) => {
        const chainlength = document.getElementById("divcustomchainlength");
        const wandlength = document.getElementById("divcustomwandlength");

        function toggleDisplay(el, show) {
            if (el) el.style.display = show ? "" : "none";
        }

        toggleDisplay(chainlength, false);
        toggleDisplay(wandlength, false);

        if (!controlType) return resolve();

        getControlName(controlType).then(controlName => {
            if (controlName === "Chain") {
                toggleDisplay(chainlength, text === "Custom");
            } else if (controlName === "Wand") {
                toggleDisplay(wandlength, text === "Custom");
            }
            resolve();
        }).catch(error => {
            resolve();
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
        "blindtype", "controltype", "colourtype", "qty", "room", "mounting", "stackposition",
        "controlposition", "chaincolour", "wandcolour", "controllength", "chainlengthvalue", "wandlengthvalue",
        "width", "drop", "fabrictype", "fabriccolour", "notes", "markup"
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
        controltype: "ControlType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        width: "Width",
        drop: "Drop",
        fabrictype: "FabricId",
        fabriccolour: "FabricColourId",
        stackposition: "StackPosition",
        controlposition: "ControlPosition",
        chaincolour: "ChainId",
        wandcolour: "WandColour",
        controllength: "ControlLength",
        chainlengthvalue: "ControlLengthValue",
        wandlengthvalue: "ControlLengthValue",
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
        "blindtype", "controltype", "colourtype", "qty", "room", "mounting", "stackposition",
        "controlposition", "chaincolour", "wandcolour", "controllength", "chainlengthvalue", "wandlengthvalue",
        "width", "drop", "fabrictype", "fabriccolour", "notes", "markup"
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
        url: "Method.aspx/DesignShadesProcess",
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

async function initDesign() {
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
        bindComponentForm("", "");
        controlForm(false);
        bindBlindType(designId);
        loader(itemAction);
        bindFabricType(designId);
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
            url: "Method.aspx/DesignDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#controltype", data.ControlTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);
        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);
        fillSelect("#chaincolour", data.ChainColours);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        setFormValues(data.ItemData);

        bindComponentForm(data.ItemData.ControlType, data.ItemData.ProductId);
        visibleCustom(data.ItemData.ControlType, data.ItemData.ControlLength);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function showInfo(type) {
    let info;

    if (type === "Control Length") {
        info = "<b>Control Length Information</b>";
        info += "<br /><br />";
        info += "- Standard";
        info += "<br />";
        info += "Our standard control length is 2/3 from your drop & maximum 1000mm for wand control.";
        info += "<br /><br />";
        info += "- Custom";
        info += "<br />";
        info += "Maximum custom control length is 1000mm form wand control";
    }

    document.getElementById("spanInfo").innerHTML = info;
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