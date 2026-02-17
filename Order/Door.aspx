<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Door.aspx.vb" Inherits="Order_Door" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Door Order" %>

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
                                                <label>Door Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Mechanism Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="tubetype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" style="display:none;">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Product ID</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="colourtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div id="divdetail">
                                            <hr />

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Quantity</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <input type="number" id="qty" class="form-control" autocomplete="off" placeholder="Quantity" value="1" />
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Room / Location</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-9 form-group">
                                                    <input type="text" id="room" class="form-control" autocomplete="off" placeholder="Room / Location" />
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Mounting</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">T</span>
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder=".... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">M</span>
                                                        <input type="number" id="widthb" class="form-control" autocomplete="off" placeholder=".... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">B</span>
                                                        <input type="number" id="widthc" class="form-control" autocomplete="off" placeholder=".... mm" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Height</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Height ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Mesh Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="meshtype" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Frame Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="framecolour" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlayoutcode">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Layout Code</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="layoutcode" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Midrail Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="midrailposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Centre of Horizontal">Centre of Horizontal</option>
                                                        <option value="Centre of Vertical">Centre of Vertical</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divhandletype">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Handle Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <select id="handletype" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Bass Latch Standard (Supply Loose)">Bass Latch Standard (Supply Loose)</option>
                                                        <option value="Bass Latch Outer Pull (Supply Loose)">Bass Latch Outer Pull (Supply Loose)</option>
                                                        <option value="Lock Key">Lock Key</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divhandlelength">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Handle Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="handlelength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divtriplelock">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Triple Lock</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="triplelock" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                        <option value="No">No</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbugseal">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Bug Seal</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bugseal" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Long Fur">Long Fur</option>
                                                        <option value="Short Fur">Short Fur</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divpetdoor">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Pet Door</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="pettype" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Small 190x240">Small 190x240</option>
                                                        <option value="Medium 225x305">Medium 225x305</option>
                                                        <option value="Large 260x400">Large 260x400</option>
                                                    </select>
                                                    <span class="text-muted">* Type</span>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="petposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Centre">Centre</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                    <span class="text-muted">* Position</span>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divdoorcloser">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Door Closer</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="doorcloser" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Angle</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="angletype" class="form-select">
                                                        <option value=""></option>
                                                        <option value="12x12mm">12x12mm</option>
                                                        <option value="12x20mm">12x20mm</option>
                                                        <option value="12x25mm">12x25mm</option>
                                                        <option value="20x20mm">20x20mm</option>
                                                        <option value="20x25mm">20x25mm</option>
                                                        <option value="20x40mm">20x40mm</option>
                                                        <option value="25x50mm">25x50mm</option>
                                                    </select>
                                                    <span class="text-muted">* Type</span>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="anglelength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                    <span class="text-muted">* Length</span>
                                                </div>
                                            </div>

                                            <div class="row" id="divbeading">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Beading</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="beading" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left-Right-Top">Left-Right-Top</option>
                                                        <option value="Left-Right">Left-Right</option>
                                                        <option value="Left-Top">Left-Top</option>
                                                        <option value="Right-Top">Right-Top</option>
                                                        <option value="Left Only">Left Only</option>
                                                        <option value="Right Only">Right Only</option>
                                                        <option value="Top Only">Top Only</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-2" id="divjambadaptor">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Jamb Adaptor</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="jambtype" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Short Leg with Fur">Short Leg with Fur</option>
                                                        <option value="Long Leg with Fur">Long Leg with Fur</option>
                                                        <option value="Short Leg">Short Leg</option>
                                                        <option value="Long Leg">Long Leg</option>
                                                    </select>
                                                    <span class="text-muted">* Type</span>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="jambposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left-Right-Top">Left-Right-Top</option>
                                                        <option value="Left-Right">Left - Right</option>
                                                        <option value="Left-Top">Left-Top</option>
                                                        <option value="Right-Top">Right-Top</option>
                                                        <option value="Left Only">Left Only</option>
                                                        <option value="Right Only">Right Only</option>
                                                    </select>
                                                    <span class="text-muted">* Position</span>
                                                </div>
                                            </div>

                                            <div class="row" id="divflushbold">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Flush Bold Location</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="flushbold" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Top & Bottom">Top & Bottom</option>
                                                        <option value="Top Only">Top Only</option>
                                                        <option value="Bottom Only">Bottom Only</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-2" id="divinterlocktype">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Interlock Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <select id="interlocktype" class="form-select">
                                                        <option value=""></option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divtoptrack">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Top Track</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="toptrack" class="form-select">
                                                        <option value=""></option>
                                                        <option value="J Track - HD1">J Track - HD1</option>
                                                        <option value="U Frame">U Frame</option>
                                                        <option value="H Track - ST4">H Track - ST4</option>
                                                        <option value="W Track - ST8">W Track - ST8</option>
                                                        <option value="P Track - ST11">P Track - ST11</option>
                                                    </select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="toptracklength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divbottomtrack">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Bottom Track</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomtrack" class="form-select">
                                                        <option value=""></option>
                                                        <option value="J Track - HD1">J Track - HD1</option>
                                                        <option value="H Track - ST4">H Track - ST4</option>
                                                        <option value="W Track - ST8">W Track - ST8</option>
                                                        <option value="P Track - ST11">P Track - ST11</option>
                                                    </select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="bottomtracklength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divreceiver">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Receiver Channel</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="receivertype" class="form-select">
                                                        <option value=""></option>
                                                        <option value="U Frame">U Frame</option>
                                                        <option value="H Track - ST4">H Track - ST4</option>
                                                    </select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="receiverlength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divslidingqty">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Sliding Roller Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="slidingqty" class="form-select">
                                                        <option value=""></option>
                                                        <option value="2 (Bottom Only)">2 (Bottom Only)</option>
                                                        <option value="4 (Top & Bottom Only)">4 (Top & Bottom Only)</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Special Information</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-9 form-group">
                                                    <textarea class="form-control" id="notes" rows="4" placeholder="Your notes ..." style="resize:none;"></textarea>
                                                </div>
                                            </div>

                                            <div class="row" id="divmarkup">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
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
                                <%--<a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery">Detail Gallery</a>--%>
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

    <script src="/Scripts/WebOrders/Door.js?v=2.0.2"></script>
</asp:Content>