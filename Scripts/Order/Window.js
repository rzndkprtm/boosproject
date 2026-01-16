let designIdOri = "20";
let itemAction;
let headerId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

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

$(document).ready(function () {
    checkSession();

    $("#submit").on("click", process);
    $("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
    $("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

    $("#blindtype").on("change", function () {
        const blindtype = $(this).val();

        bindMounting(blindtype);
        bindMeshType(blindtype);
        bindFrameColour(blindtype);

        bindColourType(blindtype);
    });

    $("#colourtype").on("change", function () {
        const colourtype = $(this).val();

        bindComponentForm(colourtype);
    });
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
                bindMeshType(selectedValue),
                bindFrameColour(selectedValue),
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
                        bindMeshType(selectedValue),
                        bindFrameColour(selectedValue),
                        bindColourType(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindMeshType(selectedValue),
                        bindFrameColour(selectedValue),
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
                bindComponentForm(blindType, selectedValue)
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

function bindMeshType(blindType) {
    return new Promise((resolve, reject) => {
        const meshtype = document.getElementById("meshtype");
        meshtype.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then((blindName) => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Safety") {
                options = [
                    { value: "", text: "" },
                    { value: "304 SS Mesh", text: "304 SS Mesh" }
                ];
            } else if (blindName === "Standard") {
                options = [
                    { value: "", text: "" },
                    { value: "Fibreglass Mesh", text: "Fibreglass Mesh" },
                    { value: "Pawproof", text: "Pawproof" },
                    { value: "SS Mesh", text: "SS Mesh" }
                ];
            } else if (blindName === "Security") {
                options = [
                    { value: "", text: "" },
                    { value: "316 SS Mesh", text: "316 SS Mesh" }
                ];
            } else if (blindName === "Flyscreen") {
                options = [
                    { value: "", text: "" },
                    { value: "Fibreglass Mesh", text: "Fibreglass Mesh" },
                    { value: "Pawproof", text: "Pawproof" },
                    { value: "SS Mesh", text: "SS Mesh" }
                ];
            }

            options.forEach((opt) => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                meshtype.appendChild(optionElement);
            });

            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function bindFrameColour(blindType) {
    return new Promise((resolve, reject) => {
        const framecolour = document.getElementById("framecolour");
        framecolour.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then((blindName) => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Safety") {
                options = [
                    { value: "", text: "" },
                    { value: "Black (Express)", text: "Black (Express)" },
                    { value: "Monument (Express)", text: "Monument (Express)" },
                    { value: "Primrose (Express)", text: "Primrose (Express)" },
                    { value: "White (Express)", text: "White (Express)" },
                    { value: "White Birch (Express)", text: "White Birch (Express)" }
                ];
            } else if (blindName === "Standard") {
                options = [
                    { value: "", text: "" },
                    { value: "Black (Express)", text: "Black (Express)" },
                    { value: "Monument (Express)", text: "Monument (Express)" },
                    { value: "Primrose (Express)", text: "Primrose (Express)" },
                    { value: "White (Express)", text: "White (Express)" },
                    { value: "White Birch (Express)", text: "White Birch (Express)" }
                ];
            } else if (blindName === "Security") {
                options = [
                    { value: "", text: "" },
                    { value: "Black (Express)", text: "Black (Express)" },
                    { value: "Monument (Express)", text: "Monument (Express)" },
                    { value: "Primrose (Express)", text: "Primrose (Express)" },
                    { value: "White (Express)", text: "White (Express)" },
                    { value: "White Birch (Express)", text: "White Birch (Express)" }
                ];
            } else if (blindName === "Flyscreen") {
                options = [
                    { value: "", text: "" },
                    { value: "Black (Express)", text: "Black (Express)" },
                    { value: "Monument (Express)", text: "Monument (Express)" },
                    { value: "Primrose (Express)", text: "Primrose (Express)" },
                    { value: "White (Express)", text: "White (Express)" },
                    { value: "White Birch (Express)", text: "White Birch (Express)" }
                ];
            }

            const extraOptions = [
                { value: "", text: "" },
                { value: "Apo Grey (Regular)", text: "Apo Grey (Regular)" },
                { value: "Beige (Regular)", text: "Beige (Regular)" },
                { value: "Birch White (Regular)", text: "Birch White (Regular)" },
                { value: "Black (Regular)", text: "Black (Regular)" },
                { value: "Brown (Regular)", text: "Brown (Regular)" },
                { value: "Charcoal (Regular)", text: "Charcoal (Regular)" },
                { value: "Deep Ocean (Regular)", text: "Deep Ocean (Regular)" },
                { value: "Dune (Regular)", text: "Dune (Regular)" },
                { value: "Hawthorne Green (Regular)", text: "Hawthorne Green (Regular)" },
                { value: "Jasper (Regular)", text: "Jasper (Regular)" },
                { value: "Monument (Regular)", text: "Monument (Regular)" },
                { value: "Notre Dame (Regular)", text: "Notre Dame (Regular)" },
                { value: "Pale Eucalypt (Regular)", text: "Pale Eucalypt (Regular)" },
                { value: "Paperbark (Regular)", text: "Paperbark (Regular)" },
                { value: "Primrose (Regular)", text: "Primrose (Regular)" },
                { value: "Silver (Regular)", text: "Silver (Regular)" },
                { value: "Surf Mist (Regular)", text: "Surf Mist (Regular)" },
                { value: "White (Regular)", text: "White (Regular)" },
                { value: "Woodland Grey (Regular)", text: "Woodland Grey (Regular)" }
            ];

            options = options.concat(extraOptions);

            options.forEach((opt) => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                framecolour.appendChild(optionElement);
            });

            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function bindComponentForm(blindType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");
        const divsToHide = [
            "divbrace", "divporthole", "divplungerpin", "divswivelcolour", "divswivelqty", "divspringqty",
            "divtopplasticqty"
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);
        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!blindType || !colourType) return resolve();

        toggleDisplay(detail, true);

        const divShow = [];

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Safety") {
                divShow.push("divbrace");
            } else if (blindName === "Standard") {
                divShow.push("divbrace");
            } else if (blindName === "Flyscreen") {
                divShow.push("divbrace", "divporthole", "divplungerpin", "divswivelcolour", "divswivelqty", "divspringqty", "divtopplasticqty");
            }

            divShow.forEach(id => {
                const el = document.getElementById(id);
                toggleDisplay(el, true);
            });

            if (typeof priceAccess !== "undefined" && priceAccess) {
                toggleDisplay(markup, true);
            }

            resolve();

        }).catch((error) => {
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
        "meshtype", "framecolour", "brace", "angletype", "anglelength", "angleqty", "porthole", "plungerpin",
        "swivelcolour", "swivelqty", "swivelqtyb", "springqty", "topplasticqty",
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
        colourtype: "ColourType",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        width: "Width",
        drop: "Drop",
        meshtype: "MeshType",
        framecolour: "FrameColour",
        brace: "Brace",
        angletype: "AngleType",
        anglelength: "AngleLength",
        angleqty: "AngleQty",
        porthole: "PortHole",
        plungerpin: "PlungerPin",
        swivelcolour: "SwivelColour",
        swivelqty: "SwivelQty",
        swivelqtyb: "SwivelQtyB",
        springqty: "SpringQty",
        topplasticqty: "TopPlasticQty",
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
        "blindtype", "colourtype", "qty", "room", "mounting", "width", "drop",
        "meshtype", "framecolour", "brace", "angletype", "anglelength", "angleqty", "porthole", "plungerpin",
        "swivelcolour", "swivelqty", "swivelqtyb", "springqty", "topplasticqty",
        "notes", "markup"
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
        url: "Method.aspx/WindowProcess",
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

async function checkSession() {
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
            url: "Method.aspx/WindowDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);

        let manual = [
            bindMeshType(data.ItemData.BlindType),
            bindFrameColour(data.ItemData.BlindType)
        ];
        await Promise.all(manual);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        bindComponentForm(data.ItemData.BlindType, data.ItemData.ProductId);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function redirectOrder() {
    window.location.replace("/order");
}