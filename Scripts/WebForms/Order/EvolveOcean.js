let designIdOri = "22";
let itemAction;
let headerId;
let itemId;
let designId;
let company;
let companyDetail;
let loginId;
let roleAccess;
let priceAccess;

initEvolveOcean();

$("#submit").on("click", process);
$("#cancel").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);
$("#vieworder").on("click", () => window.location.href = `/order/detail?orderid=${headerId}`);

$("#blindtype").on("change", function () {
    const blindtype = $(this).val();
    const midrailheight1 = parseFloat(document.getElementById("midrailheight1").value) || 0;
    const framebottom = document.getElementById("framebottom").value;

    bindColourType(blindtype);
    bindMounting(blindtype);

    bindLayoutCode(blindtype);
    bindFrameType(blindtype, mounting);
    bindBottomTrack(blindtype, framebottom);
    bindTiltrodSplit(midrailheight1);
    visibleSemiInside(blindtype, mounting);
});

$("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindComponentForm(blindtype, $(this).val());
});

$("#mounting").on("change", function () {
    const mounting = $(this).val();
    const blindtype = document.getElementById("blindtype").value;

    bindFrameType(blindtype, mounting);
    visibleSemiInside(blindtype, mounting);
});

$("#midrailheight1").on("input", function () {
    const midrailheight1 = parseFloat(document.getElementById("midrailheight1").value) || 0;
    const midrailheight2 = parseFloat(document.getElementById("midrailheight2").value) || 0;

    bindMidrailCritical(midrailheight1, midrailheight2);
    bindTiltrodSplit(midrailheight1);
});

$("#midrailheight2").on("input", function () {
    const midrailheight1 = document.getElementById("midrailheight1").value || 0;
    const midrailheight2 = document.getElementById("midrailheight2").value || 0;

    bindMidrailCritical(midrailheight1, midrailheight2);
});

$("#joinedpanels").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const hingecolour = document.getElementById("hingecolour").value;

    visibleHingeColour(blindtype, $(this).val());
    visibleHingesLoose(blindtype, hingecolour, $(this).val());
});

$("#hingecolour").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const joinedPanels = document.getElementById("joinedpanels").value;

    visibleHingesLoose(blindtype, $(this).val(), joinedPanels);
});

$("#layoutcode").on("change", function () {
    $("#layoutcodecustom").val("");
    $("#samesizepanel").val("");
    const blindtype = document.getElementById("blindtype").value;
    let layoutcode = $(this).val();

    visibleLayoutCustom(layoutcode);

    if (layoutcode === "Other") {
        layoutcode = document.getElementById("layoutcodecustom").value;
    }
    visibleSameSize(blindtype, layoutcode);
    visibleGap(blindtype, "", layoutcode);
});

$("#layoutcodecustom").on("input", function () {
    $("#samesizepanel").val("");
    const blindtype = document.getElementById("blindtype").value;
    const layoutcode = $(this).val();

    visibleSameSize(blindtype, layoutcode);
    visibleGap(blindtype, "", layoutcode);
});

$("#samesizepanel").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const layout = document.getElementById("layoutcode").value;
    const layoutcustom = document.getElementById("layoutcodecustom").value;

    let layoutcode = layout;
    if (layout === "Other") layoutcode = layoutcustom;

    visibleGap(blindtype, $(this).val(), layoutcode);
});

$("#frametype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const frametype = $(this).val();
    const mounting = document.getElementById("mounting").value;

    bindLeftFrame(frametype);
    bindRightFrame(frametype);
    bindTopFrame(frametype, mounting);
    bindBottomFrame(frametype, mounting);
    visibleFrameDetail(frametype);
    visibleBuildout(blindtype, frametype);
});

$("#framebottom").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const framebottom = $(this).val();

    bindBottomTrack(blindtype, framebottom);
    visibleBottomTrack(blindtype, framebottom);
});

$("#horizontaltpostheight").on("input", function () {
    const value = parseFloat($(this).val()) || 0;
    const horizontalrequired = document.getElementById("divHorizontalTPostRequired");

    horizontalrequired.style.display = "none";
    if (value === 0) return;
    if (value > 0) horizontalrequired.style.display = "";
});

$("#tiltrodsplit").on("change", function () {
    visibleSplitHeight($(this).val());
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
                bindColourType(selectedValue),
                bindLayoutCode(selectedValue)
            ]).then(resolve).catch(reject);
            return;
        }

        const listData = { type: "BlindTypeShutter", companydetail: companyDetail, designtype: designType };

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
                        bindLayoutCode(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    const selectedValue = blindtype.value || "";
                    Promise.all([
                        bindMounting(selectedValue),
                        bindColourType(selectedValue),
                        bindLayoutCode(selectedValue)
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

function bindMidrailCritical(height1, height2) {
    return new Promise((resolve) => {
        const midrailcritical = document.getElementById("midrailcritical");
        midrailcritical.innerHTML = "";

        visibleMidrail(height1);

        let options = [{ value: "", text: "" }];

        if (height1 > 0 && height2 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes - Top Only", text: "Yes - Top Only" },
                { value: "Yes - Bottom Only", text: "Yes - Bottom Only" },
            ];
        } else if (height1 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
            ];
        } else if (height2 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes - Top Only", text: "Yes - Top Only" },
                { value: "Yes - Bottom Only", text: "Yes - Bottom Only" },
            ];
        }

        options.forEach((opt) => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            midrailcritical.appendChild(optionElement);
        });

        resolve();
    });
}

function bindLayoutCode(blindType) {
    return new Promise((resolve, reject) => {
        const layoutcode = document.getElementById("layoutcode");
        layoutcode.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then((blindName) => {
            let options = [{ value: "", text: "" }];

            switch (blindName) {
                case "Hinged":
                    options = [
                        { value: "", text: "" },
                        { value: "L", text: "L" },
                        { value: "R", text: "R" },
                        { value: "LR", text: "LR" },
                        { value: "LD-R", text: "LD-R" },
                        { value: "L-DR", text: "L-DR" },
                        { value: "LTLR", text: "LTLR" },
                        { value: "LRTR", text: "LRTR" },
                        { value: "LRTLR", text: "LRTLR" },
                        { value: "LTLRTR", text: "LTLRTR" },
                        { value: "LD-RTLD-R", text: "LD-RTLD-R" },
                        { value: "L-DRTL-DR", text: "L-DRTL-DR" },
                        { value: "Other", text: "Other" },
                    ];
                    break;

                case "Hinged Bi-fold":
                    options = [
                        { value: "", text: "" },
                        { value: "LL", text: "LL" },
                        { value: "RR", text: "RR" },
                        { value: "LLRR", text: "LLRR" },
                        { value: "Other", text: "Other" },
                    ];
                    break;

                case "Track Bi-fold":
                    options = [
                        { value: "", text: "" },
                        { value: "LL", text: "LL" },
                        { value: "RR", text: "RR" },
                        { value: "LLRR", text: "LLRR" },
                        { value: "LLLL", text: "LLLL" },
                        { value: "RRRR", text: "RRRR" },
                        { value: "LLRRRR", text: "LLRRRR" },
                        { value: "LLLLRR", text: "LLLLRR" },
                        { value: "LLLLLL", text: "LLLLLL" },
                        { value: "RRRRRR", text: "RRRRRR" },
                        { value: "LLRRRRRR", text: "LLRRRRRR" },
                        { value: "LLLLRRRR", text: "LLLLRRRR" },
                        { value: "LLLLLLRR", text: "LLLLLLRR" },
                        { value: "LLLLLLLL", text: "LLLLLLLL" },
                        { value: "RRRRRRRR", text: "RRRRRRRR" },
                        { value: "Other", text: "Other" },
                    ];
                    break;

                case "Track Sliding":
                    options = [
                        { value: "", text: "" },
                        { value: "BF", text: "BF" },
                        { value: "FB", text: "FB" },
                        { value: "BFB", text: "BFB" },
                        { value: "FBF", text: "FBF" },
                        { value: "BFFB", text: "BFFB" },
                        { value: "FBBF", text: "FBBF" },
                        { value: "BBFF", text: "BBFF" },
                        { value: "FFBB", text: "FFBB" },
                        { value: "Other", text: "Other" },
                    ];
                    break;

                case "Track Sliding Single Track":
                    options = [
                        { value: "", text: "" },
                        { value: "F", text: "F" },
                        { value: "FF", text: "FF" },
                        { value: "FFF", text: "FFF" },
                        { value: "FFFF", text: "FFFF" },
                        { value: "Other", text: "Other" },
                    ];
                    break;

                case "Fixed":
                    options = [
                        { value: "", text: "" },
                        { value: "F", text: "F" },
                        { value: "FF", text: "FF" },
                        { value: "FFF", text: "FFF" },
                        { value: "FFFF", text: "FFFF" },
                        { value: "FFFFFF", text: "FFFFFF" },
                        { value: "Other", text: "Other" },
                    ];
                    break;
            }
            options.forEach((opt) => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                layoutcode.appendChild(optionElement);
            });
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function bindFrameType(blindType, mounting) {
    return new Promise((resolve, reject) => {
        const frametype = document.getElementById("frametype");
        frametype.innerHTML = "";

        visibleFrameDetail(frametype.value);
        visibleBuildout(blindType, frametype.value);

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then((blindName) => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
                options = [
                    { value: "", text: "" },
                    { value: "Beaded L 49mm", text: "Beaded L 49mm" },
                    { value: "Insert L 49mm", text: "Insert L 49mm" },
                    { value: "No Frame", text: "No Frame" }
                ];
                if (mounting === "Inside") {
                    options = [
                        { value: "", text: "" },
                        { value: "Beaded L 49mm", text: "Beaded L 49mm" },
                        { value: "Insert L 49mm", text: "Insert L 49mm" },
                        { value: "Small Bullnose Z Frame", text: "Small Bullnose Z Frame" },
                        { value: "Large Bullnose Z Frame", text: "Large Bullnose Z Frame" },
                        { value: "No Frame", text: "No Frame" }
                    ];
                }
            } else if (blindName === "Track Bi-fold") {
                options = [
                    { value: "", text: "" },
                    { value: "92mm", text: "92mm" },
                    { value: "152mm", text: "152mm" },
                    { value: "185mm", text: "185mm" }
                ];
            } else if (blindName === "Track Sliding") {
                options = [
                    { value: "", text: "" },
                    { value: "152mm", text: "152mm" },
                    { value: "185mm", text: "185mm" }
                ];                
            } else if (blindName === "Track Sliding Single Track") {
                options = [
                    { value: "", text: "" },
                    { value: "92mm", text: "92mm" },
                    { value: "152mm", text: "152mm" },
                    { value: "185mm", text: "185mm" }
                ];
            } else if (blindName === "Fixed") {
                options = [
                    { value: "", text: "" },
                    { value: "U Channel", text: "U Channel" },
                    { value: "19x19 Light Block", text: "19x19 Light Block" }
                ];
            }

            options.forEach((opt) => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                frametype.appendChild(optionElement);
            });

            if (frametype.options.length === 1) {
                bindLeftFrame(frametype.value, mounting);
                bindRightFrame(frametype.value, mounting);
                bindTopFrame(frametype.value, mounting);
                bindBottomFrame(frametype.value, mounting);

                visibleFrameDetail(frametype.value);
                visibleBuildout(blindType, frametype.value);
            }

            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function bindLeftFrame(frameType) {
    return new Promise((resolve) => {
        const frameleft = document.getElementById("frameleft");
        frameleft.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" },
                { value: "Sill Plate (Bullnose Z)", text: "SILL PLATE (BULLNOSE Z)" },
            ];
        } else if (frameType === "No Frame") {
            options = [
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
            ];
        } else if (frameType === "U Channel") {
            options = [
                { value: "No", text: "No" }
            ];
        } else if (frameType === "19x19 Light Block") {
            options = [
                { value: "No", text: "No" }
            ];
        }

        options.forEach((opt) => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frameleft.appendChild(optionElement);
        });

        resolve();
    });
}

function bindRightFrame(frameType) {
    return new Promise((resolve) => {
        const frameright = document.getElementById("frameright");
        frameright.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" },
                { value: "Sill Plate (Bullnose Z)", text: "Sill Plate (Bullnose Z)" },
            ];
        } else if (frameType === "No Frame") {
            options = [
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" }
            ];
        } else if (frameType === "U Channel") {
            options = [
                { value: "No", text: "No" }
            ];
        } else if (frameType === "19x19 Light Block") {
            options = [
                { value: "No", text: "No" }
            ];
        }

        options.forEach((opt) => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frameright.appendChild(optionElement);
        });

        resolve();
    });
}

function bindTopFrame(frameType, mounting) {
    return new Promise((resolve, reject) => {
        const frametop = document.getElementById("frametop");
        frametop.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" },
                { value: "Sill Plate (Bullnose Z)", text: "Sill Plate (Bullnose Z)" },
                { value: "Roller Catch Ramp", text: "Roller Catch Ramp" }
            ];
        } else if (frameType === "No Frame") {
            options = [
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" }
            ];
            if (mounting === "Inside") {
                options = [
                    { value: "Yes", text: "Yes" }
                ];
            }
        } else if (frameType === "U Channel") {
            options = [
                { value: "", text: "" },
                { value: "No", text: "No" },
                { value: "U Channel", text: "U Channel" }
            ];
        } else if (frameType === "19x19 Light Block") {
            options = [
                { value: "No", text: "No" }
            ];
        }

        options.forEach((opt) => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frametop.appendChild(optionElement);
        });
        resolve();
    });
}

function bindBottomFrame(frameType) {
    return new Promise((resolve, reject) => {
        const framebottom = document.getElementById("framebottom");
        framebottom.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" },
                { value: "Light Block", text: "Light Block" },
                { value: "Sill Plate (Bullnose Z)", text: "Sill Plate (Bullnose Z)" },
                { value: "Roller Catch Ramp", text: "Roller Catch Ramp" }
            ];
        } else if (frameType === "No Frame") {
            options = [
                { value: "Light Block", text: "Light Block" }
            ];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "Yes" },
                { value: "No", text: "No" }
            ];
        } else if (frameType === "U Channel") {
            options = [
                { value: "", text: "" },
                { value: "No", text: "No" },
                { value: "U Channel", text: "U Channel" }
            ];
        } else if (frameType === "19x19 Light Block") {
            options = [
                { value: "No", text: "No" }
            ];
        }

        options.forEach((opt) => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            framebottom.appendChild(optionElement);
        });
        resolve();
    });
}

function bindBottomTrack(blindType, bottomFrame) {
    return new Promise((resolve, reject) => {
        const bottomtracktype = document.getElementById("bottomtracktype");
        bottomtracktype.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then((blindName) => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Track Bi-fold" || blindName === "Track Sliding" || blindName === "Track Sliding Single Track") {
                options = [
                    { value: "", text: "" },
                    { value: "M Track", text: "M Track" },
                    { value: "U Track", text: "U Track" },
                ];
                if (bottomFrame === "Yes") {
                    options = [{ value: "U Track", text: "U Track" }];
                }
            }

            options.forEach((opt) => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                bottomtracktype.appendChild(optionElement);
            });

            if (bottomtracktype.options.length === 0) {
                bottomtracktype.selectedIndex = 0;
            }

            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function bindTiltrodSplit(height1) {
    return new Promise((resolve) => {
        const tiltrodsplit = document.getElementById("tiltrodsplit");
        tiltrodsplit.innerHTML = "";

        visibleSplitHeight(tiltrodsplit.value);

        let options = [{ value: "", text: "" }];

        if (height1 === 0) {
            options = [
                { value: "", text: "" },
                { value: "None", text: "None" },
                { value: "Split Halfway", text: "Split Halfway" },
                { value: "Other", text: "Other" },
            ];
        } else if (height1 > 0) {
            options = [
                { value: "", text: "" },
                { value: "None", text: "None" },
                { value: "Split Halfway Above Midrail", text: "Split Halfway Above Midrail" },
                { value: "Split Halfway Below Midrail", text: "Split Halfway Below Midrail" },
                { value: "Split Halfway Above and Below Midrail", text: "Split Halfway Above & Below Midrail" },
                { value: "Other", text: "Other" },
            ];
        }

        options.forEach((opt) => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            tiltrodsplit.appendChild(optionElement);
        });

        if (tiltrodsplit.options.length === 0) {
            tiltrodsplit.selectedIndex = 0;
            visibleSplitHeight(tiltrodsplit.value);
        }

        resolve();
    });
}

function bindComponentForm(blindType, colourType) {
    return new Promise((resolve) => {
        const detail = document.getElementById("divdetail");
        const markup = document.getElementById("divmarkup");

        const divsToHide = [
            "divlouvreposition",
            "divmidrailheight2",
            "divmidrailcritical",
            "divpanelqty",
            "divjoinedpanels",
            "divhingecolour",
            "divsemiinside",
            "divcustomheaderlength",
            "divlayoutcode",
            "divlayoutcodecustom",
            "divframetype",
            "divframeleft",
            "divframeright",
            "divframetop",
            "divframebottom",
            "divbottomtracktype",
            "divbuildout",
            "divsamesizepanel",
            "divgappost",
            "divhorizontaltpost",
            "divhorizontaltpostrequired",
            "divtiltrodtype",
            "divtiltrodsplit",
            "divtiltrodheight",
            "divreversehinged",
            "divpelmetflat",
            "divextrafascia",
            "divhingesloose"
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
            if (blindName === "Panel Only") {
                divShow.push("divpanelqty", "divtiltrodtype", "divtiltrodsplit");
            } else if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
                divShow.push("divhingecolour", "divlayoutcode", "divframetype", "divtiltrodtype", "divtiltrodsplit", "divhorizontaltpost");
            } else if (blindName === "Track Bi-fold") {
                divShow.push("divhingecolour", "divlayoutcode", "divframetype", "divtiltrodtype", "divtiltrodsplit", "divbottomtracktype", "divreversehinged", "divpelmetflat", "divextrafascia");
            } else if (blindName === "Track Sliding") {
                divShow.push("divlouvreposition", "divjoinedpanels", "divcustomheaderlength", "divlayoutcode", "divframetype", "divtiltrodtype", "divtiltrodsplit", "divbottomtracktype", "divpelmetflat", "divextrafascia");
            } else if (blindName === "Track Sliding Single Track") {
                divShow.push("divjoinedpanels", "divcustomheaderlength", "divlayoutcode", "divframetype", "divtiltrodtype", "divtiltrodsplit", "divbottomtracktype", "divpelmetflat", "divextrafascia");
            } else if (blindName === "Fixed") {
                divShow.push("divlayoutcode", "divframetype", "divtiltrodtype", "divtiltrodsplit");
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

function visibleMidrail(height1) {
    return new Promise((resolve) => {
        const divMidrailHeight2 = document.getElementById("divmidrailheight2");
        const divMidrailCritical = document.getElementById("divmidrailcritical");

        divMidrailHeight2.style.display = "none";
        divMidrailCritical.style.display = "none";

        if (height1 > 0) {
            divMidrailHeight2.style.display = "";
            divMidrailCritical.style.display = "";
        }

        resolve();
    });
}

function visibleHingeColour(blindType, joinedPanels) {
    return new Promise((resolve, reject) => {
        const divHingeColour = document.getElementById("divhingecolour");
        divHingeColour.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Hinged" || blindName === "Hinged Bi-fold" || blindName === "Track Bi-fold") {
                divHingeColour.style.display = "";
            } else if (blindName === "Track Sliding" || blindName === "Track Sliding Single Track") {
                if (joinedPanels === "Yes") {
                    divHingeColour.style.display = "";
                }
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleLayoutCustom(layout) {
    return new Promise((resolve) => {
        const divLayoutCodeCustom = document.getElementById("divlayoutcodecustom");
        divLayoutCodeCustom.style.display = "none";
        if (layout === "Other") {
            divLayoutCodeCustom.style.display = "";
        }

        resolve();
    });
}

function visibleFrameDetail(frameType) {
    return new Promise((resolve) => {
        const frameElements = [
            document.getElementById("divframeleft"),
            document.getElementById("divframeright"),
            document.getElementById("divframetop"),
            document.getElementById("divframebottom"),
        ];

        const shouldHide = frameType === "" || frameType === "19x19 Light Block";
        const displayValue = shouldHide ? "none" : "";

        frameElements.forEach((element) => {
            element.style.display = displayValue;
        });

        resolve();
    });
}

function visibleBottomTrack(blindType, frameBottom) {
    return new Promise((resolve, reject) => {
        const divBottomTrackType = document.getElementById("divBottomTrackType");
        divBottomTrackType.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Track Bi-fold" || blindName === "Track Sliding" || blindName === "Track Sliding Single Track") {
                if (frameBottom === "No") {
                    divBottomTrackType.style.display = "";
                }
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleBuildout(blindType, frameType) {
    return new Promise((resolve, reject) => {
        const divBuildout = document.getElementById("divbuildout");
        divBuildout.style.display = "none";

        if (!blindType || !frameType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if ((blindName === "Hinged" || blindName === "Hinged Bi-fold") && frameType === "Insert L 49mm") {
                divBuildout.style.display = "";
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleSameSize(blindType, layoutCode) {
    return new Promise((resolve, reject) => {
        const divSameSize = document.getElementById("divsamesizepanel");
        divSameSize.style.display = "none";

        if (!blindType && !layoutCode) return resolve();

        getBlindName(blindType).then((blindName) => {
            if ((blindName === "Hinged" || blindName === "Hinged Bi-fold" || blindName === "Track Bi-fold") && cekSameSizePanels(layoutCode)) {
                divSameSize.style.display = "";
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleGap(blindType, sameSize, layoutCode) {
    return new Promise((resolve, reject) => {
        const divGapPost = document.getElementById("divgappost");
        divGapPost.style.display = "none";

        const divGap1 = document.getElementById("divgap1");
        const divGap2 = document.getElementById("divgap2");
        const divGap3 = document.getElementById("divgap3");
        const divGap4 = document.getElementById("divgap4");
        const divGap5 = document.getElementById("divgap5");

        divGap1.style.display = "none";
        divGap2.style.display = "none";
        divGap3.style.display = "none";
        divGap4.style.display = "none";
        divGap5.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Hinged" || blindName === "Hinged Bi-fold" || blindName === "Track Bi-fold") {
                if (cekSameSizePanels(layoutCode)) {
                    if (sameSize === "Yes") {
                        divGapPost.style.display = "none";
                    } else {
                        let countT = 0;
                        for (let char of layoutCode) {
                            if (char === "T" || char === "B" || char === "C" || char === "G")
                                countT++;
                        }
                        if (countT > 0) divGapPost.style.display = "";
                        if (countT > 0) divGap1.style.display = "";
                        if (countT > 1) divGap2.style.display = "";
                        if (countT > 2) divGap3.style.display = "";
                        if (countT > 3) divGap4.style.display = "";
                        if (countT > 4) divGap5.style.display = "";
                    }
                } else {
                    let countT = 0;
                    for (let char of layoutCode) {
                        if (char === "T" || char === "B" || char === "C" || char === "G")
                            countT++;
                    }
                    if (countT > 0) divGapPost.style.display = "";
                    if (countT > 0) divGap1.style.display = "";
                    if (countT > 1) divGap2.style.display = "";
                    if (countT > 2) divGap3.style.display = "";
                    if (countT > 3) divGap4.style.display = "";
                    if (countT > 4) divGap5.style.display = "";
                }
            } else if (blindName === "Track Bi-fold") {
                let countT = 0;
                for (let char of layoutCode) {
                    if (char === "T" || char === "B" || char === "C" || char === "G")
                        countT++;
                }
                if (countT > 0) divGapPost.style.display = "";
                if (countT > 0) divGap1.style.display = "";
                if (countT > 1) divGap2.style.display = "";
                if (countT > 2) divGap3.style.display = "";
                if (countT > 3) divGap4.style.display = "";
                if (countT > 4) divGap5.style.display = "";
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleHingesLoose(blindType, hingeColour, joinedPanels) {
    return new Promise((resolve, reject) => {
        const divHingesLoose = document.getElementById("divhingesloose");
        divHingesLoose.style.display = "none";

        if (!hingeColour) return resolve();

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Hinged" || blindName === "Hinged Bi-fold" || blindName === "Track Bi-fold") {
                divHingesLoose.style.display = "";
            } else if (blindName === "Track Sliding" && joinedPanels === "Yes") {
                divHingesLoose.style.display = "";
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleSemiInside(blindType, mounting) {
    return new Promise((resolve, reject) => {
        const divSemiInside = document.getElementById("divsemiinside");
        divSemiInside.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if ((blindName === "Track Bi-fold" || blindName === "Track Sliding" || blindName === "Track Sliding Single Track") && mounting === "Inside") {
                divSemiInside.style.display = "";
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function visibleSplitHeight(tiltrodSplit) {
    return new Promise((resolve) => {
        const tiltrodHeight = document.getElementById("divtiltrodheight");
        tiltrodHeight.style.display = "none";

        if (tiltrodSplit === "Other") tiltrodHeight.style.display = "";

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

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
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
        "blindtype",
        "colourtype",
        "qty",
        "room",
        "mounting",
        "semiinside",
        "width",
        "drop",
        "louvresize",
        "louvreposition",
        "midrailheight1",
        "midrailheight2",
        "midrailcritical",
        "panelqty",
        "joinedpanels",
        "hingecolour",
        "hingesloose",
        "customheaderlength",
        "layoutcode",
        "layoutcodecustom",
        "frametype",
        "frameleft",
        "frameright",
        "frametop",
        "framebottom",
        "bottomtracktype",
        "buildout",
        "samesizepanel",
        "gap1",
        "gap2",
        "gap3",
        "gap4",
        "gap5",
        "horizontaltpostheight",
        "horizontaltpost",
        "tiltrodtype",
        "tiltrodsplit",
        "splitheight1",
        "splitheight2",
        "pelmetflat",
        "reversehinged",
        "extrafascia",
        "markup",
        "notes",
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
        semiinside: "SemiInsideMount",
        width: "Width",
        drop: "Drop",
        louvresize: "LouvreSize",
        louvreposition: "LouvrePosition",
        midrailheight1: "MidrailHeight1",
        midrailheight2: "MidrailHeight2",
        midrailcritical: "MidrailCritical",
        panelqty: "PanelQty",
        joinedpanels: "JoinedPanels",
        hingecolour: "HingeColour",
        hingesloose: "HingesLoose",
        customheaderlength: "CustomHeaderLength",
        layoutcode: "LayoutCode",
        layoutcodecustom: "LayoutCodeCustom",
        frametype: "FrameType",
        frameleft: "FrameLeft",
        frameright: "FrameRight",
        frametop: "FrameTop",
        framebottom: "FrameBottom",
        bottomtracktype: "BottomTrackType",
        buildout: "Buildout",
        samesizepanel: "SameSizePanel",
        gap1: "Gap1",
        gap2: "Gap2",
        gap3: "Gap3",
        gap4: "Gap4",
        gap5: "Gap5",
        horizontaltpostheight: "HorizontalTPostHeight",
        horizontaltpost: "HorizontalTPost",
        tiltrodtype: "TiltrodType",
        tiltrodsplit: "TiltrodSplit",
        splitheight1: "SplitHeight1",
        splitheight2: "SplitHeight2",
        reversehinged: "ReverseHinged",
        pelmetflat: "PelmetFlat",
        extrafascia: "ExtraFascia",
        notes: "Notes",
        markup: "MarkUp",
    };

    Object.keys(mapping).forEach((id) => {
        const el = document.getElementById(id);
        if (!el) {
            return;
        }

        let value = itemData[mapping[id]];
        if (id === "markup" && value === 0) value = "";
        el.value = value || "";
    });
    const maxLength = 1000;
    const notesLength = (itemData["Notes"] || "").length;
    $("#notescount").text(`${notesLength}/${maxLength}`);

    if (itemAction === "copy") {
        const resetFields = ["room", "notes"];
        resetFields.forEach((id) => {
            const el = document.getElementById(id);
            if (el) el.value = "";
        });

        $("#notescount").text(`0/${maxLength}`);
    }
}

function cekSameSizePanels(layoutCode) {
    if (layoutCode.length === 0) return false;
    if (layoutCode.includes("T")) {
        if (
            layoutCode.includes("B") ||
            layoutCode.includes("C") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("B")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("C") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("C")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("B") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("G")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("B") ||
            layoutCode.includes("C")
        ) {
            return false;
        }
    }
    return (
        layoutCode.includes("T") ||
        layoutCode.includes("B") ||
        layoutCode.includes("C") ||
        layoutCode.includes("G")
    );
}

function showInfo(type) {
    let title;
    let info;

    if (type === "Tiltrod Type") {
        info = "Tiltrod Type Information";
        info += "<br /><br />";
        info += "<b>Easy Tilt</b>: Internal rack and pinion.<br /><b>Clearview</b>: Metal rod on back edge of louvres.";
    } else if (type === "Gap") {
        info = "T-Post / Gap / Bay / Corner Location Information";
        info += "<br /><br />";
        info += "The factory will make all panels within an opening the same width unless otherwise indicated. If specific T-post locations are required, enter the measurement from the far left-hand side to the centre of the T-post measurement. The factory will make the panels to fit in between these posts.";
    }
    document.getElementById("spanInfo").innerHTML = info;
}

function process() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype",
        "colourtype",
        "qty",
        "room",
        "mounting",
        "semiinside",
        "width",
        "drop",
        "louvresize",
        "louvreposition",
        "midrailheight1",
        "midrailheight2",
        "midrailcritical",
        "panelqty",
        "joinedpanels",
        "hingecolour",
        "hingesloose",
        "customheaderlength",
        "layoutcode",
        "layoutcodecustom",
        "frametype",
        "frameleft",
        "frameright",
        "frametop",
        "framebottom",
        "bottomtracktype",
        "buildout",
        "samesizepanel",
        "gap1",
        "gap2",
        "gap3",
        "gap4",
        "gap5",
        "horizontaltpostheight",
        "horizontaltpost",
        "tiltrodtype",
        "tiltrodsplit",
        "splitheight1",
        "splitheight2",
        "reversehinged",
        "pelmetflat",
        "extrafascia",
        "markup",
        "notes",
    ];

    const formData = {
        headerid: headerId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId,
    };

    fields.forEach((id) => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/EvolveProccess",
        data: JSON.stringify({ data: formData }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d.trim();
            if (result === "Success") {
                setTimeout(() => {
                    $("#modalSuccess").modal("show");
                    startCountdown(3);
                }, 500);
            } else {
                isError(result);
                toggleButtonState(false, "Submit");
            }
        },
        error: function () {
            toggleButtonState(false, "Submit");
        },
    });
}

async function initEvolveOcean() {
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
        bindComponentForm("", "");
        controlForm(false);
        bindBlindType(designId);
        loader(itemAction);
    } else if (["edit", "view", "copy"].includes(itemAction)) {
        await bindItemOrder(itemId);
        controlForm(
            itemAction === "view",
            itemAction === "edit",
            itemAction === "copy"
        );
    }
}

async function bindItemOrder(itemId) {
    try {
        const response = await $.ajax({
            type: "POST",
            url: "Method.aspx/Detail",
            data: JSON.stringify({ itemId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        const data = response.d;
        if (!data.length) return;

        const itemData = data[0];
        const {
            BlindType: blindtype,
            ColourType: colourtype,
            Mounting: mounting,
            MidrailHeight1: height1,
            MidrailHeight2: height2,
            FrameType: frameType,
            FrameBottom: bottomFrame,
            JoinedPanels: joinedPanels,
            LayoutCode: layoutCode,
            LayoutCodeCustom: layoutCodeCustom,
            SamePanelSize: sameSize,
            HingeColour: hingeColour,
            TiltrodSplit: tiltrodSplit
        } = itemData;

        let layoutCodeFinal = layoutCode;
        if (layoutCode === "Other") {
            layoutCodeFinal = layoutCodeCustom
        }

        await bindBlindType(designId);
        await delay(100);

        await bindColourType(blindtype);
        await bindMounting(blindtype);
        await bindLayoutCode(blindtype);
        await bindMidrailCritical(height1, height2);
        await bindTiltrodSplit(height1);

        await bindFrameType(blindtype, mounting);
        await bindLeftFrame(frameType);
        await bindRightFrame(frameType);
        await bindTopFrame(frameType, mounting);
        await bindBottomFrame(frameType);

        await bindBottomTrack(blindtype, bottomFrame);

        setFormValues(itemData);

        await Promise.all([
            bindComponentForm(blindtype, colourtype),
            visibleMidrail(height1),
            visibleHingeColour(blindtype, joinedPanels),
            visibleFrameDetail(frameType),
            visibleBuildout(blindtype, frameType),
            visibleSameSize(blindtype, layoutCodeFinal),
            visibleGap(blindtype, sameSize, layoutCodeFinal),
            visibleLayoutCustom(layoutCode),
            visibleHingesLoose(blindtype, hingeColour, joinedPanels),
            visibleSemiInside(blindtype, mounting),
            visibleSplitHeight(tiltrodSplit)
        ]);

        document.getElementById("divloader").style.display = "none";
        document.getElementById("divorder").style.display = "";
    } catch (error) {
        reject(error);
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