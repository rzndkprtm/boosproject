<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SkylineExpress.aspx.vb" Inherits="Order_SkylineExpress" MasterPageFile="~/Site.Master" Title="Skyline Express Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3 id="pageaction"></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
                            <li class="breadcrumb-item"><a id="orderDetail" href="#">Detail</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row" id="divloader">
            <div class="col-12">
                <div class="card">
                    <div class="card-header text-center">
                        <h5>PREPARING DATA</h5>
                    </div>
                    <div class="card-body text-center">
                        <img runat="server" src="~/assets/vendors/svg-loaders/puff.svg" class="me-4" style="width: 3rem" alt="audio">
                        <div class="text-secondary mb-3">Please wait ....</div>
                    </div>
                </div>
            </div>
        </section>

        <div id="divorder" style="display:none;">
            <section class="row">
                <div class="col-12 col-sm-12 col-lg-7">
                    <div class="card">
                        <div class="card-content">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-6 col-sm-6 col-lg-3 mb-2">
                                        <label>Order #</label>
                                        <br />
                                        <label id="orderid" class="font-bold"></label>
                                    </div>
                                    <div class="col-6 col-sm-6 col-lg-4 mb-2">
                                        <label>Order Number</label>
                                        <br />
                                        <label id="ordernumber" class="font-bold"></label>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-5">
                                        <label>Order Name</label>
                                        <br />
                                        <label id="ordername" class="font-bold"></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <section class="row">
                <div class="col-12 col-sm-12 col-lg-7">
                    <div class="card">
                        <div class="card-header">
                            <h4 class="card-title" id="cardtitle"></h4>
                        </div>
                        <div class="card-content">
                            <div class="card-body">
                                <div class="form form-horizontal">
                                    <div class="form-body">
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Colour</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <select id="colourtype" class="form-select"></select>
                                            </div>
                                        </div>
                                        <div id="divdetail">
                                            <hr />
                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Quantity</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <input type="number" id="qty" class="form-control" autocomplete="off" placeholder="Quantity" value="1" />
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Room / Location</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-9 form-group">
                                                    <input type="text" id="room" class="form-control" autocomplete="off" placeholder="Room / Location" />
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Mounting</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divsemiinside">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Semi Inside Mount</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="semiinside" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Width x Height</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">W</span>
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">H</span>
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Louvre Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="louvresize" class="form-select">
                                                        <option value=""></option>
                                                        <option value="89">89mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlouvreposition">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Louvre Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="louvreposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Open">Open</option>
                                                        <option value="Closed">Closed</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Midrail Height</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="midrailheight1" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divmidrailheight2">
                                                    <div class="input-group">
                                                        <input type="number" id="midrailheight2" class="form-control" autocomplete="off" placeholder="Height 2 ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divmidrailcritical">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Critical Midrail</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="midrailcritical" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divpanelqty">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Panel Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="panelqty" class="form-select">
                                                        <option value=""></option>
                                                        <option value="1">1</option>
                                                        <option value="2">2</option>
                                                        <option value="3">3</option>
                                                        <option value="4">4</option>
                                                        <option value="5">5</option>
                                                        <option value="6">6</option>
                                                        <option value="7">7</option>
                                                        <option value="8">8</option>
                                                        <option value="9">9</option>
                                                        <option value="10">10</option>
                                                        <option value="11">11</option>
                                                        <option value="12">12</option>
                                                        <option value="13">13</option>
                                                        <option value="14">14</option>
                                                        <option value="15">15</option>
                                                        <option value="16">16</option>
                                                        <option value="17">17</option>
                                                        <option value="18">18</option>
                                                        <option value="19">19</option>
                                                        <option value="20">20</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divjoinedpanels">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Co-Joined Panels</label>
                                                </div>
                                                <div class="col-2 form-group">
                                                    <select id="joinedpanels" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divhingecolour">
                                                <div class="col-12 col-sm-12 col-lg-4">
                                                    <label>Hinge Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="hingecolour" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Stainless Steel">Stainless Steel</option>
                                                        <option value="White">White</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divhingesloose">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Hinges Loose</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="hingesloose" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divcustomheaderlength">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Custom Header</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="customheaderlength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divlayoutcode">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Layout Code</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="layoutcode" class="form-select"></select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-5 form-group" id="divlayoutcodecustom">
                                                    <div class="input-group">
                                                        <input type="text" id="layoutcodecustom" class="form-control" autocomplete="off" placeholder="Custom ...." />
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Layout Code');">?</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divframetype">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Frame Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="frametype" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divframeleft">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Left Frame</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="frameleft" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divframeright">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Right Frame</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="frameright" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divframetop">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Top Frame</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="frametop" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divframebottom">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Frame</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="framebottom" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-2" id="divbottomtracktype">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Track Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomtracktype" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divbottomtrackrecess">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Track Recess</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="bottomtrackrecess" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbuildout">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Buildout</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="buildout" class="form-select">
                                                        <option value=""></option>
                                                        <option value="9.5mm Buildout">9.5mm Buildout</option>
                                                        <option value="25mm Buildout">25mm Buildout</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divbuildoutposition">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Buildout Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="buildoutposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Back of Frame">Back of Frame</option>
                                                        <option value="Back of Lip">Back of Lip</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsamesizepanel">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Same Size Panel</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="samesizepanel" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="mb-2 row" id="divgappost">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>GAP / T-Post</label>
                                                </div>
                                                <div class="col-9">
                                                    <div class="row mb-3">
                                                        <div class="col-12 col-sm-12 col-lg-4" id="divgap1">
                                                            <div class="input-group">
                                                                <input type="number" id="gap1" class="form-control" autocomplete="off" placeholder="... mm" />
                                                                <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');">?</a>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12 col-lg-4" id="divgap2">
                                                            <div class="input-group">
                                                                <input type="number" id="gap2" class="form-control" autocomplete="off" placeholder="... mm" />
                                                                <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');">?</a>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12 col-lg-4" id="divgap3">
                                                            <div class="input-group">
                                                                <input type="number" id="gap3" class="form-control" autocomplete="off" placeholder="... mm" />
                                                                <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');">?</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mb-3">
                                                        <div class="col-12 col-sm-12 col-lg-4" id="divgap4">
                                                            <div class="input-group">
                                                                <input type="number" id="gap4" class="form-control" autocomplete="off" placeholder="... mm" />
                                                                <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');">?</a>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12 col-lg-4" id="divgap5">
                                                            <div class="input-group">
                                                                <input type="number" id="gap5" class="form-control" autocomplete="off" placeholder="... mm" />
                                                                <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');">?</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <%--DISINI GAP--%>

                                            <div class="row mt-3" id="divhorizontaltpost">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Horizontal T-Post</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="horizontaltpostheight" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divhorizontaltpostrequired">
                                                    <div class="input-group">
                                                        <select class="form-select" id="horizontaltpost">
                                                            <option value=""></option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No Post">No Post</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divtiltrodtype">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Tiltrod Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="tiltrodtype" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Easy Tilt">Easy Tilt</option>
                                                            <option value="Clearview">Clearview</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Tiltrod Type');">?</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divtiltrodsplit">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Split Tiltrod</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <div class="input-group">
                                                        <select id="tiltrodsplit" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divtiltrodheight">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Split Height</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">1st</span>
                                                        <input type="number" id="splitheight1" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">2nd</span>
                                                        <input type="number" id="splitheight2" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divreversehinged">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Reverse Hinged</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="reversehinged" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divpelmetflat">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Pelmet Flat Packed</label>
                                                </div>
                                                <div class="col-2 form-group">
                                                    <select id="pelmetflat" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divextrafascia">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Extra Fascia</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="extrafascia" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>
                                
                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Special Information</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-9 form-group">
                                                    <textarea class="form-control" id="notes" rows="4" placeholder="Your notes ..." style="resize:none;"></textarea>
                                                </div>
                                            </div>

                                            <div class="row" id="divmarkup">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Mark Up</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="markup" class="form-control" autocomplete="off" placeholder="Mark Up ..." />
                                                        <span class="input-group-text">%</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <a href="javascript:void(0);" id="submit" class="btn btn-primary">Submit</a>
                            <a href="javascript:void(0);" id="cancel" class="btn btn-danger">Cancel</a>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-lg-5">
                    <div class="card">
                        <div class="card-header">
                            <h4 class="card-title text-center">Information</h4>
                        </div>
                        <div class="card-content">
                            <div class="card-body">
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>

    <div class="modal fade text-left" id="modalSuccess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Successfully</h5>
                </div>
                <div class="modal-body">
                    Your order has been successfully saved
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" id="vieworder" class="btn btn-success w-100" data-bs-dismiss="modal">View Order</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalError" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger ">
                    <h5 class="modal-title white text-center">Information</h5>
                </div>

                <div class="modal-body text-center py-4">
                    <span id="errorMsg"></span>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalInfo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Information</h5>
                </div>
                <div class="modal-body">
                    <span id="spanInfo"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalGallery" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-body">
                    <asp:Image runat="server" CssClass="w-100" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    
    <script src="<%: ResolveUrl("~/Scripts/WebOrders/SkylineExpress.js?v=1.0.3") %>"></script>
</asp:Content>