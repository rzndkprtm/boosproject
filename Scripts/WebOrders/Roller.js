let designIdOri = "12";
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

initRoller();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    bindTubeType($(this).val());
    bindMounting($(this).val());
});

$("#tubetype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;

    bindControlType(blindtype, $(this).val());
    bindFabricType(designId);
});

$("#controltype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const tubetype = document.getElementById("tubetype").value;

    bindColourType(blindtype, tubetype, $(this).val());
    bindChainRemote(designId, blindtype, $(this).val());
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const tubetype = document.getElementById("tubetype").value;
    const controltype = document.getElementById("controltype").value;

    visibleDetail(blindtype, tubetype, controltype, $(this).val());
});

$("#fabrictype").on("change", function () {
    bindFabricColour($(this).val());
});

$("#fabrictypeb").on("change", function () {
    bindFabricColourB($(this).val());
});

$("#fabrictypec").on("change", function () {
    bindFabricColourC($(this).val());
});

$("#fabrictyped").on("change", function () {
    bindFabricColourD($(this).val());
});

$("#fabrictypee").on("change", function () {
    bindFabricColourE($(this).val());
});

$("#fabrictypef").on("change", function () {
    bindFabricColourF($(this).val());
});

$("#bottomtype").on("change", function () {
    bindBottomColour($(this).val());

    visibleBottomColour(1, $(this).val());
    visibleFlatBottom($(this).val(), 1);
});

$("#bottomtypeb").on("change", function () {
    bindBottomColourB($(this).val());

    visibleBottomColour(2, $(this).val());
    visibleFlatBottom($(this).val(), 2);
});

$("#bottomtypec").on("change", function () {
    bindBottomColourC($(this).val());

    visibleBottomColour(3, $(this).val());
    visibleFlatBottom($(this).val(), 3);
});

$("#bottomtyped").on("change", function () {
    bindBottomColourD($(this).val());

    visibleBottomColour(4, $(this).val());
    visibleFlatBottom($(this).val(), 4);
});

$("#bottomtypee").on("change", function () {
    bindBottomColourE(designId, $(this).val());
    visibleFlatBottom($(this).val(), 5);
});

$("#bottomtypef").on("change", function () {
    bindBottomColourF(designId, $(this).val());
    visibleFlatBottom($(this).val(), 6);
});

$("#chaincolour").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    const controllength = document.getElementById("controllength").value;

    bindChainStopper($(this).val());
    visibleChainStopperLength(controltype, $(this).val(), 1);
    visibleCustomChainLength($(this).val(), controllength, 1);
});

$("#chaincolourb").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    const controllength = document.getElementById("controllengthb").value;

    bindChainStopperB($(this).val());
    visibleChainStopperLength(controltype, $(this).val(), 2);
    visibleCustomChainLength($(this).val(), controllength, 2);
});

$("#chaincolourc").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    const controllength = document.getElementById("controllengthc").value;

    bindChainStopperC($(this).val());
    visibleChainStopperLength(controltype, $(this).val(), 3);
    visibleCustomChainLength($(this).val(), controllength, 3);
});

$("#chaincolourd").on("change", function () {
    const controltype = document.getElementById("controltype").value;
    const controllength = document.getElementById("controllengthd").value;

    bindChainStopperD($(this).val());
    visibleChainStopperLength(controltype, $(this).val(), 4);
    visibleCustomChainLength($(this).val(), controllength, 4);
});

$("#controllength").on("change", function () {
    const chaincolour = document.getElementById("chaincolour").value;
    visibleCustomChainLength(chaincolour, $(this).val(), 1);
});

$("#controllengthb").on("change", function () {
    const chaincolour = document.getElementById("chaincolourb").value;
    visibleCustomChainLength(chaincolour, $(this).val(), 2);
});

$("#controllengthc").on("change", function () {
    const chaincolour = document.getElementById("chaincolourc").value;
    visibleCustomChainLength(chaincolour, $(this).val(), 3);
});

$("#controllengthd").on("change", function () {
    const chaincolour = document.getElementById("chaincolourd").value;
    visibleCustomChainLength(chaincolour, $(this).val(), 4);
});

$("#width").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 1, $(this).val());
});

$("#widthb").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 2, $(this).val());
});

$("#widthc").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 3, $(this).val());
});

$("#widthd").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 4, $(this).val());
});

$("#widthe").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 5, $(this).val());
});

$("#widthf").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisWidth(blindtype, 6, $(this).val());
});

$("#drop").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 1, $(this).val());
});

$("#dropb").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 2, $(this).val());
});

$("#dropc").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 3, $(this).val());
});

$("#dropd").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 4, $(this).val());
});

$("#drope").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 5, $(this).val());
});

$("#dropf").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;
    otomatisDrop(blindtype, 6, $(this).val());
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

function getCompanyDetailName(companyDetail) {
    if (!companyDetail) return;

    const type = "CompanyDetailName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: companyDetail }),
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

function getTubeName(tubeType) {
    if (!tubeType) return;

    const type = "TubeName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: tubeType }),
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

function getBottomName(bottomType) {
    if (!bottomType) return Promise.resolve("");

    const type = "BottomName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: bottomType }),
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

function getChainLength(chainColour) {
    if (!chainColour) return;

    const type = "ChainLength";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: chainColour }),
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
                bindTubeType(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "BlindTypeRoller", companydetail: companyDetail, designtype: designType };
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
                bindControlType(blindType, selectedValue),
                bindFabricType(designId)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "TubeType", companydetail: companyDetail, blindtype: blindType };

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
                        bindControlType(blindType, selectedValue),
                        bindFabricType(designId)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = tubetype.value || "";
                    Promise.all([
                        bindControlType(blindType, selectedValue),
                        bindFabricType(designId)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindControlType(blindType, tubeType) {
    return new Promise((resolve, reject) => {
        const controltype = document.getElementById("controltype");
        controltype.innerHTML = "";

        if (!blindType || !tubeType) {
            const selectedValue = controltype.value || "";
            Promise.all([
                bindColourType(blindType, tubeType, selectedValue),
                bindChainRemote(designId, blindType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        let listData = { type: "ControlType", companydetail: companyDetail, blindtype: blindType, tubetype: tubeType };

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
                        bindColourType(blindType, tubeType, selectedValue),
                        bindChainRemote(designId, blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = controltype.value || "";
                    Promise.all([
                        bindColourType(blindType, tubeType, selectedValue),
                        bindChainRemote(designId, blindType, selectedValue)
                    ]).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindColourType(blindType, tubeType, controlType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType || !tubeType || !controlType) {
            const selectedValue = colourtype.value || "";
            Promise.all([
                visibleDetail(blindType, tubeType, controlType, selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "ColourType", companydetail: companyDetail, blindtype: blindType, tubetype: tubeType, controltype: controlType };

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
                        visibleDetail(blindType, tubeType, controlType, selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = colourtype.value || "";
                    Promise.all([
                        visibleDetail(blindType, tubeType, controlType, selectedValue)
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

function bindChainRemote(designType, blindType, controlType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["chaincolour", "chaincolourb", "chaincolourc", "chaincolourd", "chaincoloure", "chaincolourf", "remote"];

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        if (!designType || !blindType || !controlType) {
            resolve();
            return;
        }

        let chainCustom = "";

        getBlindName(blindType).then(blindName => {
            if (blindName === "Full Cassette" || blindName === "Semi Cassette") {
                chainCustom = "Cassette";
            }

            const listData = {
                type: "ControlColour",
                designtype: designType,
                controltype: controlType,
                companydetail: companyDetail,
                customtype: chainCustom
            };

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
                    }
                    resolve();
                },
                error: function (error) {
                    reject(error);
                }
            });
        }).catch(error => reject(error));
    });
}

function bindChainStopper(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopper = document.getElementById("chainstopper");
        chainstopper.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopper.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopper.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopper.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopper.selectedIndex = 0;
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

function bindChainStopperB(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopperb = document.getElementById("chainstopperb");
        chainstopperb.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopperb.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopperb.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopperb.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopperb.selectedIndex = 0;
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

function bindChainStopperC(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopperc = document.getElementById("chainstopperc");
        chainstopperc.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopperc.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopperc.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopperc.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopperc.selectedIndex = 0;
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

function bindChainStopperD(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopperd = document.getElementById("chainstopperd");
        chainstopperd.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopperd.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopperd.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopperd.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopperd.selectedIndex = 0;
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

function bindChainStopperE(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstoppere = document.getElementById("chainstoppere");
        chainstoppere.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstoppere.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstoppere.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstoppere.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstoppere.selectedIndex = 0;
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

function bindChainStopperF(chainColour) {
    return new Promise((resolve, reject) => {
        const chainstopperf = document.getElementById("chainstopperf");
        chainstopperf.innerHTML = "";

        if (!chainColour) {
            resolve();
            return;
        }

        const listData = { type: "ChainStopper", chaincolour: chainColour };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    chainstopperf.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        chainstopperf.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        chainstopperf.add(option);
                    });

                    if (response.d.length === 1) {
                        chainstopperf.selectedIndex = 0;
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
        const typeIds = ["fabrictype", "fabrictypeb", "fabrictypec", "fabrictyped", "fabrictypee", "fabrictypef"];
        const bindFunctions = [bindFabricColour, bindFabricColourB, bindFabricColourC, bindFabricColourD, bindFabricColourE, bindFabricColourF];

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

        const listData = {
            type: "FabricTypeByDesign",
            designtype: designType,
            companydetail: companyDetail
        };

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

function bindFabricColourB(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolourb = document.getElementById("fabriccolourb");
        fabriccolourb.innerHTML = "";

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

function bindFabricColourC(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolourc = document.getElementById("fabriccolourc");
        fabriccolourc.innerHTML = "";

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
                    fabriccolourc.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolourc.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolourc.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolourc.selectedIndex = 0;
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

function bindFabricColourD(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolourd = document.getElementById("fabriccolourd");
        fabriccolourd.innerHTML = "";

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
                    fabriccolourd.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolourd.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolourd.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolourd.selectedIndex = 0;
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

function bindFabricColourE(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccoloure = document.getElementById("fabriccoloure");
        fabriccoloure.innerHTML = "";

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
                    fabriccoloure.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccoloure.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccoloure.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccoloure.selectedIndex = 0;
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

function bindFabricColourF(fabricType) {
    return new Promise((resolve, reject) => {
        const fabriccolourf = document.getElementById("fabriccolourf");
        fabriccolourf.innerHTML = "";

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
                    fabriccolourf.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        fabriccolourf.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        fabriccolourf.add(option);
                    });

                    if (response.d.length === 1) {
                        fabriccolourf.selectedIndex = 0;
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

function bindBottomType(designType) {
    return new Promise((resolve, reject) => {
        const typeIds = ["bottomtype", "bottomtypeb", "bottomtypec", "bottomtyped", "bottomtypee", "bottomtypef"];
        const bindFunctions = [bindBottomColour, bindBottomColourB, bindBottomColourC, bindBottomColourD, bindBottomColourE, bindBottomColourF];
        const visibleFunctions = typeIds.map((_, i) => (val) => visibleFlatBottom(val, i + 1));

        typeIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) select.innerHTML = "";
        });

        const callBottomVisibility = (idx, val) => {
            const blindNumber = idx + 1;
            return Promise.all([
                visibleFunctions[idx](val),
                visibleBottomColour(blindNumber, val)
            ]);
        };

        const callBindAndVisibility = (dataValue) => {
            return typeIds.map((id, idx) => {
                const val = document.getElementById(id)?.value || "";
                return bindFunctions[idx](dataValue, val)
                    .then(() => callBottomVisibility(idx, val));
            });
        };

        if (!designType) {
            Promise.all(callBindAndVisibility("")).then(resolve).catch(reject);
            return;
        }

        const listData = {
            type: "BottomType",
            designtype: designType,
            companydetail: companyDetail
        };

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

                    Promise.all(callBindAndVisibility(designType)).then(resolve).catch(reject);
                } else {
                    Promise.all(callBindAndVisibility(designType)).then(resolve).catch(reject);
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindBottomColour(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolour = document.getElementById("bottomcolour");
        bottomcolour.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolour.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolour.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolour.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolour.selectedIndex = 0;
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

function bindBottomColourB(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolourb = document.getElementById("bottomcolourb");
        bottomcolourb.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolourb.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolourb.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolourb.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolourb.selectedIndex = 0;
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

function bindBottomColourC(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolourc = document.getElementById("bottomcolourc");
        bottomcolourc.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolourc.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolourc.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolourc.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolourc.selectedIndex = 0;
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

function bindBottomColourD(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolourd = document.getElementById("bottomcolourd");
        bottomcolourd.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolourd.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolourd.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolourd.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolourd.selectedIndex = 0;
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

function bindBottomColourE(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcoloure = document.getElementById("bottomcoloure");
        bottomcoloure.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcoloure.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcoloure.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcoloure.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcoloure.selectedIndex = 0;
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

function bindBottomColourF(bottomType) {
    return new Promise((resolve, reject) => {
        const bottomcolourf = document.getElementById("bottomcolourf");
        bottomcolourf.innerHTML = "";

        if (!bottomType) {
            resolve();
            return;
        }

        let listData = { type: "BottomColour", bottomtype: bottomType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    bottomcolourf.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        bottomcolourf.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        bottomcolourf.add(option);
                    });

                    if (response.d.length === 1) {
                        bottomcolourf.selectedIndex = 0;
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

function visibleDetail(blindType, tubeType, controlType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divdbfront", "divdbback",
            "divlinkindfirst", "divlinkdepfirst", "divfirstend",
            "divsecond", "divlinkindsecond", "divlinkdepsecond", "divsecondend",
            "divthird", "divlinkindthird", "divlinkdepthird", "divthirdend",
            "divfourth", "divlinkindfourth", "divlinkdepfourth", "divfourthend",
            "divfifth", "divlinkindfifth", "divlinkdepfifth", "divfifthend",
            "divsixth", "divlinkindsixth", "divlinkdepsixth", "divsixthend",
            "divcontrolposition", "divcontrolpositionb", "divcontrolpositionc", "divcontrolpositiond", "divcontrole", "divcontrolf",
            "divcontrollength", "divcontrollengthb", "divcontrollengthc", "divcontrollengthd", "divcontrollengthe", "divcontrollengthf",
            "divcontrollengthvalue", "divcontrollengthvalueb", "divcontrollengthvaluec", "divcontrollengthvalued", "divcontrollengthvaluee", "divcontrollengthvaluef",
            "divcontrollengthvalue2", "divcontrollengthvalueb2", "divcontrollengthvaluec2", "divcontrollengthvalued2", "divcontrollengthvaluee2", "divcontrollengthvaluef2",
            "divremote", "divcharger", "divextensioncable", "divsupply",
            "divchaincolour", "divchaincolourb", "divchaincolourc", "divchaincolourd", "divchaincoloure", "divchaincolourf",
            "divchainstopper", "divchainstopperb", "divchainstopperc", "divchainstopperd", "divchainstoppere", "divchainstopperf",
            "divfabric", "divfabricb", "divfabricc", "divfabricd", "divfabrice", "divfabricf",
            "divroll", "divrollb", "divrollc", "divrolld", "divrolle", "divrollf",
            "divbottomtype", "divbottomtypeb", "divbottomtypec", "divbottomtyped", "divbottomtypee", "divbottomtypef",
            "divbottomcolour", "divbottomcolourb", "divbottomcolourc", "divbottomcolourd", "divbottomcoloure", "divbottomcolourf",
            "divflatbottom", "divflatbottomb", "divflatbottomc", "divflatbottomd", "divflatbottome", "divflatbottomf",
            "divsize", "divsizeb", "divsizec", "divsized", "divsizee", "divsizef",
            "divtoptrack", "divspringassist", "divbracketsize", "divbracketextension", "divadjusting", "divmarkup", "divprinting"
        ].map(id => document.getElementById(id));

        const toggleDisplay = (el, show) => {
            if (el) el.style.display = show ? "" : "none";
        };

        toggleDisplay(detail, false);
        toggleDisplay(markup, false);
        divsToHide.forEach(el => toggleDisplay(el, false));

        if (!blindType || !tubeType || !controlType || !colourType) return resolve();

        toggleDisplay(detail, true);

        Promise.all([
            getBlindName(blindType),
            getTubeName(tubeType),
            getControlName(controlType),
            getCompanyDetailName(companyDetail)
        ]).then(([blindName, tubeName, controlName, companyDetailName]) => {
            const divShow = [];

            const textdbfront = document.getElementById("textdbfront");
            const textdbback = document.getElementById("textdbback");

            const textlinkindfirst = document.getElementById("textlinkindfirst");
            const textlinkindsecond = document.getElementById("textlinkindsecond");
            const textlinkindthird = document.getElementById("textlinkindthird");
            const textlinkindfourth = document.getElementById("textlinkindfourth");
            const textlinkindfifth = document.getElementById("textlinkindfifth");
            const textlinkindsixth = document.getElementById("textlinkindsixth");

            const textlinkdepfirst = document.getElementById("textlinkdepfirst");
            const textlinkdepsecond = document.getElementById("textlinkdepsecond");
            const textlinkdepthird = document.getElementById("textlinkdepthird");
            const textlinkdepfourth = document.getElementById("textlinkdepfourth");
            const textlinkdepfifth = document.getElementById("textlinkdepfifth");
            const textlinkdepsixth = document.getElementById("textlinkdepsixth");

            const textthird = document.getElementById("textthird");
            const textfourth = document.getElementById("textfourth");
            const textfifth = document.getElementById("textfifth");
            const textsixth = document.getElementById("textsixth");

            if (blindName === "Single Blind") {
                divShow.push("divfabric", "divroll", "divcontrolposition", "divbottomtype", "divsize");
                if (companyDetailName === "ACCENT") {
                    divShow.push("divtoptrack");
                }
                if (["Gear Reduction 38mm", "Gear Reduction 45mm", "Gear Reduction 49mm"].includes(tubeName)) {
                    divShow.push("divbracketextension");
                }
            }
            else if (blindName === "Wire Guide") {
                divShow.push("divfabric", "divcontrolposition", "divbottomtype", "divsize");
                document.getElementById("roll").value = "Standard";
            }
            else if (blindName === "Full Cassette" || blindName === "Semi Cassette") {
                divShow.push("divfabric", "divcontrolposition", "divsize");
                document.getElementById("roll").value = "Standard";
            }
            else if (blindName === "Dual Blinds") {
                divShow.push(
                    "divdbfront", "divdbback",
                    "divfirstend", "divsecondend",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divfabricb", "divrollb", "divcontrolpositionb",
                    "divbottomtypeb", "divsizeb"
                );
                textdbfront.innerHTML = "FIRST ROLLER";
                textdbback.innerHTML = "SECOND ROLLER";

            }
            else if (blindName === "Link 2 Blinds Dependent") {
                divShow.push(
                    "divlinkdepfirst", "divfirstend",
                    "divlinkdepsecond", "divsecondend",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb"
                );
                if (["Gear Reduction 38mm", "Gear Reduction 45mm", "Gear Reduction 49mm"].includes(tubeName)) {
                    divShow.push("divbracketextension");
                }
                textlinkdepfirst.innerHTML = "FIRST BLIND / CONTROL BLIND / BLIND NO 1";
                textlinkdepsecond.innerHTML = "SECOND BLIND / END BLIND / BLIND NO 2";
            }
            else if (blindName === "Link 2 Blinds Independent") {
                divShow.push(
                    "divlinkindfirst", "divfirstend",
                    "divlinkindsecond", "divsecondend",
                    "divfabric", "divroll",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb"
                );
                if (["Gear Reduction 38mm", "Gear Reduction 45mm", "Gear Reduction 49mm"].includes(tubeName)) {
                    divShow.push("divbracketextension");
                }
                textlinkindfirst.innerHTML = "FIRST BLIND & LEFT CONTROL BLIND";
                textlinkindsecond.innerHTML = "SECOND BLIND & RIGHT CONTROL BLIND";
            }
            else if (blindName === "Link 3 Blinds Dependent") {
                divShow.push(
                    "divlinkdepfirst", "divfirstend",
                    "divlinkdepsecond", "divsecondend",
                    "divlinkdepthird", "divthirdend",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divbottomtypec", "divsizec"
                );
                if (["Gear Reduction 38mm", "Gear Reduction 45mm", "Gear Reduction 49mm"].includes(tubeName)) {
                    divShow.push("divbracketextension");
                }
                textlinkdepfirst.innerHTML = "FIRST BLIND / CONTROL BLIND / BLIND NO 1";
                textlinkdepsecond.innerHTML = "SECOND BLIND / MIDDLE BLIND / BLIND NO 2";
                textlinkdepthird.innerHTML = "THIRD BLIND / END BLIND / BLIND NO 3";
            }
            else if (blindName === "Link 3 Blinds Independent with Dependent") {
                divShow.push(
                    "divlinkindfirst", "divfirstend",
                    "divlinkindsecond", "divsecondend",
                    "divlinkindthird", "divthirdend",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divbottomtypec", "divsizec"
                );
                if (["Gear Reduction 38mm", "Gear Reduction 45mm", "Gear Reduction 49mm"].includes(tubeName)) {
                    divShow.push("divbracketextension");
                }

                textlinkindfirst.innerHTML = "FIRST BLIND / INDEPENDENT CONTROL";
                textlinkindsecond.innerHTML = "SECOND BLIND / MIDDLE BLIND";
                textlinkindthird.innerHTML = "THIRD BLIND / SECOND CONTROL";
            }
            else if (blindName === "DB Link 2 Blinds Dependent") {
                divShow.push(
                    "divdbfront",
                    "divfirstend", "divsecondend",
                    "divlinkdepfirst", "divfirstend",
                    "divlinkdepsecond", "divsecondend",
                    "divlinkdepthird", "divthirdend",
                    "divlinkdepfourth", "divfourthend",
                    "divthird",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divfabricc", "divrollc", "divcontrolpositionc",
                    "divbottomtypec", "divsizec",
                    "divbottomtyped", "divsized"
                );
                textdbfront.innerHTML = "ROLLER - FIRST SIDE";
                textlinkdepfirst.innerHTML = "First Blind / Control Blind / Blind No 1";
                textlinkdepsecond.innerHTML = "Second Blind / End Blind / Blind No 2";

                textthird.innerHTML = "ROLLER - SECOND SIDE";
                textlinkdepthird.innerHTML = "Third Blind / Control Blind / Blind No 3";
                textlinkdepfourth.innerHTML = "Fourth Blind / End Blind / Blind No 4";
            }
            else if (blindName === "DB Link 2 Blinds Independent") {
                divShow.push(
                    "divdbfront",
                    "divfirstend", "divsecondend",
                    "divlinkindfirst", "divfirstend",
                    "divlinkindsecond", "divsecondend",
                    "divlinkindthird", "divthirdend",
                    "divlinkindfourth", "divfourthend",
                    "divthird",
                    "divfabric", "divroll",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divfabricc", "divrollc",
                    "divbottomtypec", "divsizec",
                    "divbottomtyped", "divsized"
                );
                textdbfront.innerHTML = "ROLLER - FIRST SIDE";
                textlinkindfirst.innerHTML = "First Blind / Blind No 1";
                textlinkindsecond.innerHTML = "Second Blind / Blind NNoO 2";

                textthird.innerHTML = "ROLLER - SECOND SIDE";
                textlinkindthird.innerHTML = "Third Blind / Blind No 3";
                textlinkindfourth.innerHTML = "Fourth Blind / Blind No 4";
            }
            else if (blindName === "DB Link 3 Blinds Dependent") {
                divShow.push(
                    "divdbfront",
                    "divfirstend", "divsecondend",
                    "divlinkdepfirst", "divfirstend",
                    "divlinkdepsecond", "divsecondend",
                    "divlinkdepthird", "divthirdend",
                    "divlinkdepfourth", "divfourthend",
                    "divlinkdepfifth", "divfifthend",
                    "divlinkdepsixth", "divsixthend",
                    "divfourth",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divbottomtypec", "divsizec",
                    "divfabricd", "divrolld", "divcontrolpositiond",
                    "divbottomtyped", "divsized",
                    "divbottomtypee", "divsizee",
                    "divbottomtypef", "divsizef"
                );
                textdbfront.innerHTML = "ROLLER - FIRST SIDE";
                textlinkdepfirst.innerHTML = "First Blind / Control Blind / Blind No 1";
                textlinkdepsecond.innerHTML = "Second Blind / Middle Blind / Blind No 2";
                textlinkdepthird.innerHTML = "Third Blind / End Blind / Blind No 3";

                textfourth.innerHTML = "ROLLER - SECOND SIDE";
                textlinkdepfourth.innerHTML = "Fourth Blind / Control Blind / Blind No 4";
                textlinkdepfifth.innerHTML = "Fifth Blind / Middle Blind / Blind No 5";
                textlinkdepsixth.innerHTML = "Sixth Blind / End Blind / Blind No 6";
            }
            else if (blindName === "DB Link 3 Blinds Independent with Dependent") {
                divShow.push(
                    "divdbfront",
                    "divfirstend", "divsecondend",
                    "divlinkindfirst", "divfirstend",
                    "divlinkindsecond", "divsecondend",
                    "divlinkindthird", "divthirdend",
                    "divlinkindfourth", "divfourthend",
                    "divlinkindfifth", "divfifthend",
                    "divlinkindsixth", "divsixthend",
                    "divfourth",
                    "divfabric", "divroll", "divcontrolposition",
                    "divbottomtype", "divsize",
                    "divbottomtypeb", "divsizeb",
                    "divbottomtypec", "divsizec",
                    "divfabricd", "divrolld", "divcontrolpositiond",
                    "divbottomtyped", "divsized",
                    "divbottomtypee", "divsizee",
                    "divbottomtypef", "divsizef"
                );
                textdbfront.innerHTML = "ROLLER - FIRST SIDE";
                textlinkindfirst.innerHTML = "First Blind / Ind Control Blind / Blind No 1";
                textlinkindsecond.innerHTML = "Second Blind / Middle Blind / Blind No 2";
                textlinkindthird.innerHTML = "Third Blind / End Blind / Blind No 3";

                textfourth.innerHTML = "ROLLER - SECOND SIDE";
                textlinkindfourth.innerHTML = "Fourth Blind / Ind Control Blind / Blind No 4";
                textlinkindfifth.innerHTML = "Fifth Blind / Middle Blind / Blind No 5";
                textlinkindsixth.innerHTML = "Sixth Blind / End Blind / Blind No 6";
            }

            if (tubeName === "Sunboss 43mm" || tubeName === "Sunboss 50mm") {
                divShow.push("divbracketsize");
                if (blindName === "Link 2 Blinds Dependent" || blindName === "Link 2 Blinds Independent" || blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                    divShow.push("divadjusting");
                }
            }

            if (tubeName === "Acmeda 49mm" && controlName === "Chain") {
                divShow.push("divspringassist");
            }

            if (controlName === "Chain") {
                divShow.push("divchaincolour");
                if (blindName === "Dual Blinds" || blindName === "Link 2 Blinds Independent" || blindName === "DB Link 2 Blinds Independent") {
                    divShow.push("divchaincolourb");
                }
                if (["Link 3 Blinds Independent with Dependent", "DB Link 2 Blinds Dependent", "DB Link 2 Blinds Independent", "DB Link 3 Blinds Independent with Dependent"].includes(blindName)) {
                    divShow.push("divchaincolourc");
                }
                if (["DB Link 2 Blinds Independent", "DB Link 3 Blinds Dependent", "DB Link 3 Blinds Independent with Dependent"].includes(blindName)) {
                    divShow.push("divchaincolourd");
                }
                if (blindName === "DB Link 3 Blinds Independent with Dependent") {
                    divShow.push("divchaincolourf");
                }
            }

            if (["Alpha 1Nm WF", "Alpha 2Nm WF", "Alpha 3Nm WF", "Alpha 5Nm HW", "Altus", "Mercure", "Sonesse 30 WF", "LSN40", "Basics Wand", "E6", "M6", "Zero Q 1.1", "Zero Q 2.0"].includes(controlName)) {
                divShow.push("divremote");
            }
            if (["Alpha 1Nm WF", "Alpha 2Nm WF", "Alpha 3Nm WF", "Alpha 5Nm HW", "Sonesse 30 WF"].includes(controlName)) {
                divShow.push("divcharger");
            }

            if (["Alpha 1Nm WF", "Alpha 2Nm WF", "Alpha 3Nm WF", "Alpha 5Nm HW"].includes(controlName)) {
                divShow.push("divcharger");
                if (companyDetailName === "BIG" || companyDetailName === "JPMD" || companyDetailName === "JPMD BP" || companyDetailName === "CWS") {
                    divShow.push("divextensioncable", "divsupply");
                }
            }

            divShow.forEach(id => toggleDisplay(document.getElementById(id), true));

            if (typeof priceAccess !== "undefined" && priceAccess) {
                toggleDisplay(markup, true);
            }

            resolve();
        }).catch(error => {
            resolve();
        });
    });
}

function visibleChainStopperLength(controlType, chainColour, number) {
    return new Promise((resolve, reject) => {
        let thisDiv = null;
        let thisDiv2 = null;
        if (number === 1) {
            thisDiv = document.getElementById("divchainstopper");
            thisDiv2 = document.getElementById("divcontrollength");
        } else if (number === 2) {
            thisDiv = document.getElementById("divchainstopperb");
            thisDiv2 = document.getElementById("divcontrollengthb");
        } else if (number === 3) {
            thisDiv = document.getElementById("divchainstopperc");
            thisDiv2 = document.getElementById("divcontrollengthc");
        } else if (number === 4) {
            thisDiv = document.getElementById("divchainstopperd");
            thisDiv2 = document.getElementById("divcontrollengthd");
        }

        if (!thisDiv || !thisDiv2) return resolve();

        thisDiv.style.display = "none";
        thisDiv2.style.display = "none";

        if (!chainColour) return resolve();

        getControlName(controlType).then(controlName => {
            if (controlName === "Chain") {
                thisDiv.style.display = "";
                thisDiv2.style.display = "";
            }
            resolve();
        }).catch(error => {
            resolve();
        });
    });
}

function visibleCustomChainLength(chainColour, chainLength, number) {
    return new Promise((resolve, reject) => {
        let thisDiv = null;
        let thisDiv2 = null;

        if (number === 1) {
            thisDiv = document.getElementById("divcontrollengthvalue");
            thisDiv2 = document.getElementById("divcontrollengthvalue2");
        } else if (number === 2) {
            thisDiv = document.getElementById("divcontrollengthvalueb");
            thisDiv2 = document.getElementById("divcontrollengthvalueb2");
        } else if (number === 3) {
            thisDiv = document.getElementById("divcontrollengthvaluec");
            thisDiv2 = document.getElementById("divcontrollengthvaluec2");
        } else if (number === 4) {
            thisDiv = document.getElementById("divcontrollengthvalued");
            thisDiv2 = document.getElementById("divcontrollengthvalued2");
        }

        if (!thisDiv || !thisDiv2) {
            return resolve();
        }

        thisDiv.style.display = "none";
        thisDiv2.style.display = "none";

        if (chainLength === "Custom") {
            getChainLength(chainColour).then(chainType => {
                if (chainType === "Continuous") {
                    thisDiv.style.display = "none";
                    thisDiv2.style.display = "";
                } else if (chainType === "Non Continuous") {
                    thisDiv.style.display = "";
                    thisDiv2.style.display = "none";
                } else {
                    thisDiv.style.display = "";
                    thisDiv2.style.display = "";
                }
                resolve();
            }).catch(error => {
                resolve();
            });
        } else {
            resolve();
        }
    });
}

function visibleBottomColour(blindNumber, bottomType) {
    return new Promise((resolve, reject) => {
        let divBottomColour;

        switch (blindNumber) {
            case 1: divBottomColour = document.getElementById("divbottomcolour"); break;
            case 2: divBottomColour = document.getElementById("divbottomcolourb"); break;
            case 3: divBottomColour = document.getElementById("divbottomcolourc"); break;
            case 4: divBottomColour = document.getElementById("divbottomcolourd"); break;
            case 5: divBottomColour = document.getElementById("divbottomcoloure"); break;
            case 6: divBottomColour = document.getElementById("divbottomcolourf"); break;
        }

        if (!divBottomColour) {
            resolve();
            return;
        }

        divBottomColour.style.display = bottomType ? "" : "none";
        resolve();
    });
}

function visibleFlatBottom(bottomType, number) {
    return new Promise((resolve, reject) => {
        let thisDiv;

        switch (number) {
            case 1: thisDiv = document.getElementById("divflatbottom"); break;
            case 2: thisDiv = document.getElementById("divflatbottomb"); break;
            case 3: thisDiv = document.getElementById("divflatbottomc"); break;
            case 4: thisDiv = document.getElementById("divflatbottomd"); break;
            case 5: thisDiv = document.getElementById("divflatbottome"); break;
            case 6: thisDiv = document.getElementById("divflatbottomf"); break;
        }

        if (!thisDiv) {
            reject();
            return;
        }

        thisDiv.style.display = "none";

        getBottomName(bottomType).then(bottomName => {
            if (bottomName === "Flat" || bottomName === "Fabric Wrap") {
                thisDiv.style.display = "";
            }
            resolve();
        }).catch(error => {
            resolve();
        });
    });
}

function otomatisWidth(blindType, blindNumber, width) {
    return new Promise((resolve, reject) => {
        if (!blindType || !blindNumber) {
            return resolve();
        }

        getBlindName(blindType).then(blindName => {
            if (blindNumber === 1) {
                if (blindName === "Dual Blinds") {
                    document.getElementById("widthb").value = width;
                } else if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("widthc").value = width;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("widthd").value = width;
                }
            } else if (blindNumber === 2) {
                if (blindName === "Dual Blinds") {
                    document.getElementById("width").value = width;
                } else if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("widthd").value = width;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("widthe").value = width;
                }
            } else if (blindNumber === 3) {
                if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("width").value = width;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("widthf").value = width;
                }
            } else if (blindNumber === 4) {
                if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("widthb").value = width;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("width").value = width;
                }
            } else if (blindNumber === 5) {
                if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("widthb").value = width;
                }
            } else if (blindNumber === 6) {
                if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("widthc").value = width;
                }
            }
            resolve();
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
            if (blindNumber === 1) {
                if (blindName === "Dual Blinds" || blindName === "Link 2 Blinds Dependent" || blindName === "Link 2 Blinds Independent") {
                    document.getElementById("dropb").value = drop;
                } else if (blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropc").value = drop;
                } else if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                    document.getElementById("drope").value = drop;
                    document.getElementById("dropf").value = drop;
                }
            } else if (blindNumber === 2) {
                if (blindName === "Dual Blinds" || blindName === "Link 2 Blinds Dependent" || blindName === "Link 2 Blinds Independent") {
                    document.getElementById("drop").value = drop;
                } else if (blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropc").value = drop;
                } else if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                    document.getElementById("drope").value = drop;
                    document.getElementById("dropf").value = drop;
                }
            } else if (blindNumber === 3) {
                if (blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropb").value = drop;
                } else if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropd").value = drop;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropd").value = drop;
                    document.getElementById("drope").value = drop;
                    document.getElementById("dropf").value = drop;
                }
            } else if (blindNumber === 4) {
                if (blindName === "DB Link 2 Blinds Dependent" || blindName === "DB Link 2 Blinds Independent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropb").value = drop;
                } else if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropb").value = drop;
                    document.getElementById("drope").value = drop;
                    document.getElementById("dropf").value = drop;
                }
            } else if (blindNumber === 5) {
                if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                    document.getElementById("dropf").value = drop;
                }
            } else if (blindNumber === 6) {
                if (blindName === "DB Link 3 Blinds Dependent" || blindName === "DB Link 3 Blinds Independent with Dependent") {
                    document.getElementById("drop").value = drop;
                    document.getElementById("dropb").value = drop;
                    document.getElementById("dropc").value = drop;
                    document.getElementById("dropd").value = drop;
                    document.getElementById("drope").value = drop;
                }
            }
            resolve();
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
            setTimeout(updateButton, 500);
        } else {
            window.location.href = `/order/detail?orderid=${headerId}`;
        }
    }
    updateButton();
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) isEditItem = false;
    if (isCopyItem === undefined) isCopyItem = false;

    document.getElementById("submit").style.display = status ? "none" : "";

    const inputs = [
        "blindtype", "tubetype", "controltype", "colourtype",
        "qty", "room", "mounting",
        "fabrictype", "fabrictypeb", "fabrictypec", "fabrictyped", "fabrictypee", "fabrictypef",
        "fabriccolour", "fabriccolourb", "fabriccolourc", "fabriccolourd", "fabriccoloure", "fabriccolourf",
        "roll", "rollb", "rollc", "rolld", "rolle", "rollf",
        "controlposition", "controlpositionb", "controlpositionc", "controlpositiond", "controlpositione", "controlpositionf",
        "remote", "charger", "extensioncable", "supply",
        "chaincolour", "chaincolourb", "chaincolourc", "chaincolourd", "chaincoloure", "chaincolourf",
        "chainstopper", "chainstopperb", "chainstopperc", "chainstopperd", "chainstoppere", "chainstopperf",
        "controllength", "controllengthb", "controllengthc", "controllengthd", "controllengthe", "controllengthf",
        "controllengthvalue", "controllengthvalueb", "controllengthvaluec", "controllengthvalued", "controllengthvaluee", "controllengthvaluef",
        "controllengthvalue2", "controllengthvalueb2", "controllengthvaluec2", "controllengthvalued2", "controllengthvaluee2", "controllengthvaluef2",
        "bottomtype", "bottomtypeb", "bottomtypec", "bottomtyped", "bottomtypee", "bottomtypef",
        "bottomcolour", "bottomcolourb", "bottomcolourc", "bottomcolourd", "bottomcoloure", "bottomcolourf",
        "bottomoption", "bottomoptionb", "bottomoptionc", "bottomoptiond", "bottomoptione", "bottomoptionf",
        "width", "widthb", "widthc", "widthd", "widthe", "widthf",
        "drop", "dropb", "dropc", "dropd", "drope", "dropf",
        "toptrack", "springassist", "bracketsize", "bracketextension", "adjusting", "markup", "notes",
        "printing", "printingb", "printingc", "printingd", "printinge", "printingf", "printingg", "printingh"
    ];

    const installerDisabledIds = [
        "tubetype",
        "controltype",
        "colourtype",
        "room",
        "fabrictype", "fabriccolour",
        "fabrictypeb", "fabriccolourb",
        "fabrictypec", "fabriccolourc",
        "fabrictyped", "fabriccolourd",
        "fabrictypee", "fabriccoloure",
        "fabrictypef", "fabriccolourf",
        "chaincolour", "chainstopper",
        "bottomtype", "bottomcolour",
        "bracketextension"
    ];

    inputs.forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;

        if (isCopyItem) {
            el.disabled = (id === "blindtype");
            return;
        }

        if (isEditItem) {
            if (id === "qty" || id === "blindtype") {
                el.disabled = true;
            } else {
                el.disabled = false;
            }

            if (roleAccess === "Installer" && installerDisabledIds.includes(id)) {
                el.disabled = true;
            }

            return;
        }

        el.disabled = status;
    });
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        tubetype: "TubeType",
        controltype: "ControlType",
        colourtype: "ProductId",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        fabrictype: "FabricId", fabrictypeb: "FabricIdB", fabrictypec: "FabricIdC", fabrictyped: "FabricIdD", fabrictypee: "FabricIdE", fabrictypef: "FabricIdF",
        fabriccolour: "FabricColourId", fabriccolourb: "FabricColourIdB", fabriccolourc: "FabricColourIdC", fabriccolourd: "FabricColourIdD", fabriccoloure: "FabricColourIdE", fabriccolourf: "FabricColourIdF",
        roll: "Roll", rollb: "RollB", rollc: "RollC", rolld: "RollD", rolle: "RollE", rollf: "RollF",
        controlposition: "ControlPosition", controlpositionb: "ControlPositionB", controlpositionc: "ControlPositionC", controlpositiond: "ControlPositionD", controlpositione: "ControlPositionE", controlpositionf: "ControlPositionF",
        remote: "ChainId",
        charger: "Charger",
        extensioncable: "ExtensionCable",
        supply: "Supply",
        chaincolour: "ChainId", chaincolourb: "ChainIdB", chaincolourc: "ChainIdC", chaincolourd: "ChainIdD", chaincoloure: "ChainIdE", chaincolourf: "ChainIdF",
        chainstopper: "ChainStopper", chainstopperb: "ChainStopperB", chainstopperc: "ChainStopperC", chainstopperd: "ChainStopperD", chainstoppere: "ChainStopperE", chainstopperf: "ChainStopperF",
        controllength: "ControlLength", controllengthb: "ControlLengthB", controllengthc: "ControlLengthC", controllengthd: "ControlLengthD", controllengthe: "ControlLengthE", controllengthf: "ControlLengthF",
        controllengthvalue: "ControlLengthValue", controllengthvalueb: "ControlLengthValueB", controllengthvaluec: "ControlLengthValueC", controllengthvalueD: "ControlLengthValueD", ontrollengthvaluee: "ControlLengthValueE", ontrollengthvaluef: "ControlLengthValueF",
        controllengthvalue2: "ControlLengthValue", controllengthvalueb2: "ControlLengthValue", controllengthvalue2c: "ControlLengthValue", controllengthvalued2: "ControlLengthValueD", controllengthvaluee2: "ControlLengthValueE", controllengthvaluef2: "ControlLengthValueF",
        bottomtype: "BottomId", bottomtypeb: "BottomIdB", bottomtypec: "BottomIdC", bottomtyped: "BottomIdD", bottomtypee: "BottomIdE", bottomtypef: "BottomIdF",
        bottomcolour: "BottomColourId", bottomcolourb: "BottomColourIdB", bottomcolourc: "BottomColourIdC", bottomcolourd: "BottomColourIdD", bottomcoloure: "BottomColourIdE", bottomcolourf: "BottomColourIdF",
        bottomoption: "FlatOption", bottomoptionb: "FlatOptionB", bottomoptionc: "FlatOptionC", bottomoptiond: "FlatOptionD", bottomoptione: "FlatOptionE", bottomoptionf: "FlatOptionF",
        width: "Width", widthb: "WidthB", widthc: "WidthC", widthd: "WidthD", widthe: "WidthE", widthf: "WidthF",
        drop: "Drop", dropb: "DropB", dropc: "DropC", dropd: "DropD", drope: "DropE", dropf: "DropF",
        toptrack: "TopTrack",
        bracketsize: "BracketSize",
        bracketextension: "BracketExtension",
        springassist: "SpringAssist",
        adjusting: "Adjusting",
        notes: "Notes",
        markup: "MarkUp",
        printing: "Printing", printingb: "PrintingB", printingc: "PrintingC", printingd: "PrintingD", printinge: "PrintingE", printingf: "PrintingF", printingg: "PrintingG", printingh: "PrintingH"
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
        "blindtype", "tubetype", "controltype", "colourtype",
        "qty", "room", "mounting",
        "fabrictype", "fabrictypeb", "fabrictypec", "fabrictyped", "fabrictypee", "fabrictypef",
        "fabriccolour", "fabriccolourb", "fabriccolourc", "fabriccolourd", "fabriccoloure", "fabriccolourf",
        "roll", "rollb", "rollc", "rolld", "rolle", "rollf",
        "controlposition", "controlpositionb", "controlpositionc", "controlpositiond", "controlpositione", "controlpositionf",
        "remote", "charger", "extensioncable", "supply",
        "chaincolour", "chaincolourb", "chaincolourc", "chaincolourd", "chaincoloure", "chaincolourf",
        "chainstopper", "chainstopperb", "chainstopperc", "chainstopperd", "chainstoppere", "chainstopperf",
        "controllength", "controllengthb", "controllengthc", "controllengthd", "controllengthe", "controllengthf",
        "controllengthvalue", "controllengthvalueb", "controllengthvaluec", "controllengthvalued", "controllengthvaluee", "controllengthvaluef",
        "controllengthvalue2", "controllengthvalueb2", "controllengthvaluec2", "controllengthvalued2", "controllengthvaluee2", "controllengthvaluef2",
        "bottomtype", "bottomtypeb", "bottomtypec", "bottomtyped", "bottomtypee", "bottomtypef",
        "bottomcolour", "bottomcolourb", "bottomcolourc", "bottomcolourd", "bottomcoloure", "bottomcolourf",
        "bottomoption", "bottomoptionb", "bottomoptionc", "bottomoptiond", "bottomoptione", "bottomoptionf",
        "width", "widthb", "widthc", "widthd", "widthe", "widthf",
        "drop", "dropb", "dropc", "dropd", "drope", "dropf",
        "toptrack", "springassist", "bracketsize", "bracketextension", "adjusting", "markup", "notes",
        "printing", "printingb", "printingc", "printingd", "printinge", "printingf", "printingg", "printingh"
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
        url: "Method.aspx/RollerProcess",
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

async function initRoller() {
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
        visibleDetail("", "", "", "");
        controlForm(false);
        bindBlindType(designId);
        bindBottomType(designId);
        loader(itemAction)
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
        document.getElementById("divloader").style.display = "";

        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/RollerDetail",
            data: JSON.stringify({ itemId, companyDetailId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;

        fillSelect("#blindtype", data.BlindTypes);
        fillSelect("#tubetype", data.TubeTypes);
        fillSelect("#controltype", data.ControlTypes);
        fillSelect("#colourtype", data.ColourTypes);
        fillSelect("#mounting", data.Mountings);

        fillSelect("#fabrictype", data.Fabrics);
        fillSelect("#fabrictypeb", data.Fabrics);
        fillSelect("#fabrictypec", data.Fabrics);
        fillSelect("#fabrictyped", data.Fabrics);
        fillSelect("#fabrictypee", data.Fabrics);
        fillSelect("#fabrictypef", data.Fabrics);
        fillSelect("#fabriccolour", data.FabricColours);
        fillSelect("#fabriccolourb", data.FabricColoursB);
        fillSelect("#fabriccolourc", data.FabricColoursC);
        fillSelect("#fabriccolourd", data.FabricColoursD);
        fillSelect("#fabriccoloure", data.FabricColoursE);
        fillSelect("#fabriccolourf", data.FabricColoursF);
        fillSelect("#bottomtype", data.Bottoms);
        fillSelect("#bottomtypeb", data.Bottoms);
        fillSelect("#bottomtypec", data.Bottoms);
        fillSelect("#bottomtyped", data.Bottoms);
        fillSelect("#bottomtypee", data.Bottoms);
        fillSelect("#bottomtypef", data.Bottoms);
        fillSelect("#bottomcolour", data.BottomColours);
        fillSelect("#bottomcolourb", data.BottomColoursB);
        fillSelect("#bottomcolourc", data.BottomColoursC);
        fillSelect("#bottomcolourd", data.BottomColoursD);
        fillSelect("#bottomcoloure", data.BottomColoursE);
        fillSelect("#bottomcolourf", data.BottomColoursF);

        fillSelect("#remote", data.Chains);
        fillSelect("#chaincolour", data.Chains);
        fillSelect("#chaincolourb", data.Chains);
        fillSelect("#chaincolourc", data.Chains);
        fillSelect("#chaincolourd", data.Chains);
        fillSelect("#chaincoloure", data.Chains);
        fillSelect("#chaincolourf", data.Chains);

        fillSelect("#chainstopper", data.Stoppers);
        fillSelect("#chainstopperb", data.StoppersB);
        fillSelect("#chainstopperc", data.StoppersC);
        fillSelect("#chainstopperd", data.StoppersD);
        fillSelect("#chainstoppere", data.StoppersE);
        fillSelect("#chainstopperf", data.StoppersF);

        setFormValues(data.ItemData);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";

        visibleDetail(data.ItemData.BlindType, data.ItemData.TubeType, data.ItemData.ControlType, data.ItemData.ProductId);

        visibleBottomColour(1, data.ItemData.BottomId);
        visibleFlatBottom(data.ItemData.BottomId, 1);
        visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainId, 1);
        visibleCustomChainLength(data.ItemData.ChainId, data.ItemData.ControlLength, 1);

        let visiblePromises = [
            visibleBottomColour(2, data.ItemData.BottomIdB),
            visibleFlatBottom(data.ItemData.BottomIdB, 2),
            visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainIdB, 2),
            visibleCustomChainLength(data.ItemData.ChainIdB, data.ItemData.ControlLengthB, 2),

            visibleBottomColour(3, data.ItemData.BottomIdC),
            visibleFlatBottom(data.ItemData.BottomIdC, 3),
            visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainIdC, 3),
            visibleCustomChainLength(data.ItemData.ChainIdC, data.ItemData.ControlLengthC, 3),

            visibleBottomColour(4, data.ItemData.BottomIdD),
            visibleFlatBottom(data.ItemData.BottomIdD, 4),
            visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainIdD, 4),
            visibleCustomChainLength(data.ItemData.ChainIdD, data.ItemData.ControlLengthD, 4),

            visibleBottomColour(5, data.ItemData.BottomIdE),
            visibleFlatBottom(data.ItemData.BottomIdE, 5),
            visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainIdE, 5),
            visibleCustomChainLength(data.ItemData.ChainIdE, data.ItemData.ControlLengthE, 5),

            visibleBottomColour(6, data.ItemData.BottomIdF),
            visibleFlatBottom(data.ItemData.BottomIdF, 6),
            visibleChainStopperLength(data.ItemData.ControlType, data.ItemData.ChainIdF, 6),
            visibleCustomChainLength(data.ItemData.ChainIdF, data.ItemData.ControlLengthF, 6)
        ];

        await Promise.all(visiblePromises);
    } catch (error) {
        document.getElementById("divloader").style.display = "none";
    }
}

function showInfo(type) {
    let title;
    let info;

    const blindtype = document.getElementById("blindtype");

    Promise.all([
        getBlindName(blindtype.value),
        getCompanyDetailName(companyDetail)
    ]).then(function ([blindName, companyDetailName]) {
        if (type === "Layout") {
            title = "Layout Information";
            info += "";
        }
        else if (type === "Roll") {
            title = "Roll Information";

            
        }
        else if (type === "Bracket Extension") {
            title = "Bracket Extension Information";
        }
        else if (type === "Chain") {
            title = "Chain Information";

            info = "<b>- Chain Colour</b>";
            info += "<br />";
            info += "<b>Stainless Steel</b>. A surcharge will be applied.";
        }

        else if (type === "Second Chain" || type === "Third Chain") {
            title = "Chain Information";

            info = "<b>- Chain Colour</b>";
            info += "<br />";
            info += "<b>Stainless Steel</b>. A surcharge will be applied.";
            info += "<br /><br />";
            info += "<b>- Chain Colour & Stopper</b>";
            info += "<br />";
            info += "If you leave this section blank, the system automates this selection the same as the selection on the first blind !";
        }
        else if (type === "Bottom") {
            title = "Bottom Rail Information";

            if (companyDetailName === "JPMD" || companyDetailName === "JPMD BP") {
                info = "<b>- Bottom Type</b> : <span style='color:red;'>Deluxe Flat</span>, <span style='color:red;'>Deluxe Oval</span> & <span style='color:red;'>Flat Mohair</span>";
                info += "<br />";
                info += "A surcharge will be applied.";
            } else if (companyDetailName === "CWS") {
                info = "<b>- Bottom Type</b> : <span style='color:red;'><span style='color:red;'>Flat Mohair</span>";
                info += "<br />";
                info += "A surcharge will be applied.";
            } else if (companyDetailName === "ACCENT") {
                info = "<b>- Bottom Type</b> : <span style='color:red;'><span style='color:red;'>Flat Mohair</span>";
                info += "<br />";
                info += "A surcharge will be applied.";
            }
        }
        else if (type === "Second Bottom" || type === "Third Bottom") {
            title = "Bottom Rail Information";

            if (companyDetailName === "JPMD" || companyDetailName === "JPMD BP") {
                info = "<b>- Bottom Type</b> : <span style='color:red;'>Deluxe Flat</span>, <span style='color:red;'>Deluxe Oval</span> & <span style='color:red;'>Flat Mohair</span>";
                info += "<br />";
                info += "A surcharge will be applied.";
            } else if (companyDetailName === "ACCENT" || companyDetailName === "OASIS") {
                info = "<b>- Bottom Type</b> : <span style='color:red;'><span style='color:red;'>Flat Mohair</span>";
                info += "<br />";
                info += "A surcharge will be applied.";
            }
            info += "<br /><br />";
            info += "<b>- Bottom Type & Colour</b>";
            info += "<br />";
            info += "If you leave this section blank, the system automates this selection the same as the selection on the first blind !";
        }
        else if (type === "Chain Length") {
            title = "Chain Length Information";

            info = "<b>- Standard</b>";
            info += "<br /><br />";
            info += "<b>- Custom</b>";
            info += "<br />";
        }
        else if (type === "Second Size") {
            title = "Second Size Information";

            if (blindName === "Dual Blinds") {
                info = "<b>Dual Blinds</b>";
                info += "<br /><br />";
                info += "If you leave this section blank or incomplete, the system automates this selection the same as the selection on the first blind.";
                info += "<br /><br />";
                info += "And if you change this size, then the size of the first blind will follow this size.";
            }
            else if (blindName === "Link 2 Blinds Dependent" || blindName === "Link 2 Blinds Independent") {
                info = "<b>Linked Blinds</b>";
                info += "<br /><br />";
                info += "If you modify the drop size in this section, the drop of the first blind will automatically follow this value.";
            }
            else if (blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                info = "<b>Linked Blinds</b>";
                info += "<br /><br />";
                info += "If you modify the drop size in this section, the drop of the first and third blinds will automatically follow this value.";
            }
        }
        else if (type === "Third Size") {
            title = "Third Size Information";

            if (blindName === "Link 3 Blinds Dependent" || blindName === "Link 3 Blinds Independent with Dependent") {
                info = "<b>Linked Blinds</b>";
                info += "<br /><br />";
                info += "If you modify the drop size in this section, the drop of the first and second blinds will automatically follow this value.";
            }
        }

        document.getElementById("titleInfo").innerHTML = title || "";
        document.getElementById("spanInfo").innerHTML = info || "";
    }).catch(function (error) {
        document.getElementById("titleInfo").innerHTML = "";
        document.getElementById("spanInfo").innerHTML = "";
    });
}

function showGallery(type) {
    let info;

    if (type === "Roll") {
        let urlImage = "https://bigblinds.ordersblindonline.com/assets/images/products/roller/roll.jpg";
        info = `<img src="${urlImage}" alt="Roll Image" style="max-width:100%;height:auto;">`;
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

document.getElementById("modalInfoTube").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalLayout").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});