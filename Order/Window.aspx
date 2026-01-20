<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Window.aspx.vb" Inherits="Order_Window" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Window Order" %>

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
                                    <div class="col-12 col-sm-12 col-lg-3">
                                        <label>Order #</label>
                                        <br />
                                        <label id="orderid" class="font-bold"></label>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-4">
                                        <label>Order Number</label>
                                        <br />
                                        <label id="ordernumber" class="font-bold"></label>
                                    </div>
                                    <div class="col-6 col-sm-6 col-lg-5">
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
                                                <label>Window Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" style="display:none;">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Window Product</label>
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
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                        <span class="input-group-text">mm</span>
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

                                            <div class="row mt-3" id="divbrace">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Brace / Joiner Height</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="brace" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Centre of Horizontal">Centre of Horizontal</option>
                                                        <option value="Centre of Vertical">Centre of Vertical</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Angle Type</label>
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
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Angle Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="anglelength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Angle Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <input type="number" id="angleqty" class="form-control" autocomplete="off" placeholder="Qty ...." />
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divporthole">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Screen Port Hole</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="porthole" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Supply Loose">Supply Loose</option>
                                                        <option value="Fitted (Diagram)">Fitted (Diagram)</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divplungerpin">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Plunger Pin</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="plungerpin" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Metal Loose (4)">Metal Loose (4)</option>
                                                        <option value="Metal Loose (6)">Metal Loose (6)</option>
                                                        <option value="Plain Loose (4)">Plain Loose (4)</option>
                                                        <option value="Plain Loose (6)">Plain Loose (6)</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divswivelcolour">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Swivel Clip Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="swivelcolour" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Black">Black</option>
                                                        <option value="White">White</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divswivelqty">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Swivel Clip Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">1.6MM</span>
                                                        <input type="number" id="swivelqty" class="form-control" autocomplete="off" placeholder="Qty ...." />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">11MM</span>
                                                        <input type="number" id="swivelqtyb" class="form-control" autocomplete="off" placeholder="Qty ...." />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divspringqty">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Spring Clip Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <input type="number" id="springqty" class="form-control" autocomplete="off" placeholder="Qty ...." />
                                                </div>
                                            </div>
                                            <div class="row" id="divtopplasticqty">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Top Clip Plastic Qty</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <input type="number" id="topplasticqty" class="form-control" autocomplete="off" placeholder="Qty ...." />
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
                            <div class="card-body"></div>
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
                    <h5 class="modal-title white text-center">System Message</h5>
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

    <script src="<%: ResolveUrl("~/Scripts/WebForms/Order/Window.js?v=1.0.0") %>"></script>
</asp:Content>