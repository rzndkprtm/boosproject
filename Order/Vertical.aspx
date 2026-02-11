<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Vertical.aspx.vb" Inherits="Order_Vertical" MasterPageFile="~/Site.master" Title="Vertical Order" %>

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
                                                <label>Vertical Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" id="divtubetype">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Slat Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <select id="tubetype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" id="divcontroltype">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Control Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <select id="controltype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" id="divcolourtype">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Headrail Colour</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
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

                                            <div class="row" id="divmounting">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Mounting</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mb-3 mt-3" id="divfabricinsert">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Fabric Insert</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="fabricinsert" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Yes">Yes</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Fabric Insert');"> ? </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabric">
                                                <div class="row mt-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabrictype" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabriccolour" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divqtyblade">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Blade Quantity</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <input type="number" id="qtyblade" class="form-control" autocomplete="off" placeholder="Blade" />
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divwidth">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>
                                            
                                            <div class="row" id="divdrop">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Drop</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Drop  ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divstackposition">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Stack Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="stackposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                        <option value="Centre">Centre</option>
                                                        <option value="Split">Split</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divcontrolposition">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincolour">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="chaincolour" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divwandcolour">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Wand Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="wandcolour" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Beige">Beige</option>
                                                        <option value="Black">Black</option>
                                                        <option value="Grey">Grey</option>
                                                        <option value="Ivory">Ivory</option>
                                                        <option value="White">White</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollength">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthoption">
                                                    <div class="input-group">
                                                        <select id="controllength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Control Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvalue">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalue" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomjoining">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Joining</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomjoining" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Chainless">Chainless</option>
                                                            <option value="With Chain">With Chain</option>
                                                            <option value="Sewn In">Sewn In</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Bottom Joining');"> ? </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divbracketextension">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Extension Bracket</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="bracketextension" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Yes">Yes</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Extension Bracket');"> ? </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divsloping">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Sloping</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="sloping" class="form-select">
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
                                <b>LIMITATIONS</b>
                                <ul>
                                    <li>Minimum width : 300mm</li>
                                    <li>Maximum width for 89mm Slat : 5511mm</li>
                                    <li>Maximum width for 127mm Slat : 5477mm</li>
                                    <li>Maximum drop : 3050mm</li>
                                </ul>
                                <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery">Detail Gallery</a>
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
                    <asp:Image runat="server" CssClass="w-100" ImageUrl="~/Assets/images/products/vertical/gallery.jpg" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    
    <script src="/Scripts/WebOrders/Vertical.js?v=2.0.0"></script>
</asp:Content>