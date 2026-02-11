<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Aluminium.aspx.vb" Inherits="Order_Aluminium" MasterPageFile="~/Site.Master" Title="Aluminium Blind Order" %>

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
                <div class="col-12 col-sm-12 col-md-12 col-lg-7">
                    <div class="card">
                        <div class="card-header">
                            <h4 class="card-title" id="cardtitle"></h4>
                        </div>
                        <div class="card-content">
                            <div class="card-body">
                                <div class="form form-horizontal">
                                    <div class="form-body">
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                <label>Aluminium Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                <label>Colour Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <select id="colourtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                <label>Aluminium Sub Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <div class="input-group">
                                                    <select id="subtype" class="form-select"></select>
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalLayout"> ? </a>
                                                </div>
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
                                        
                                            <div class="row" id="divfirstblind">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">First Blind / Blind A</div>
                                                    </div>
                                                </div>
                                            </div>
                                        
                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>
                                        
                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Drop</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Drop ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divcontrolposition">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlposition" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divtilterposition">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Tilter Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="tilterposition" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcordlength">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Cord Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Cord Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcordlengthvalue">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalue" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divwandlength">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Wand Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="wandlength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Wand Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divwandlengthvalue">
                                                    <div class="input-group">
                                                        <input type="number" id="wandlengthvalue" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divfirstend">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">
                                                            <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('First');">Open Layout (First Blind)</a>
                                                        </div>
                                                    
                                                    </div>
                                                </div>
                                            </div>
                                        
                                            <div class="row" id="divsecondblind">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">Second Blind / Blind B</div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divwidthb">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="widthb" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divdropb">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Drop</label>
                                                </div>
                                                <div class="col-7 col-sm-7 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="dropb" class="form-control" autocomplete="off" placeholder="Drop ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-2 form-group">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Second Drop');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divcordlengthb">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Cord Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Cord Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcordlengthvalueb">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalueb" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divwandlengthb">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Wand Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="wandlengthb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Wand Length');"> ? </a>
                                                    </div>
                                                </div>
                                            
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divwandlengthvalueb">
                                                    <div class="input-group">
                                                        <input type="number" id="wandlengthvalueb" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divsecondend">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">
                                                            <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Second');">Open Layout (Second Blind)</a>
                                                        </div>
            
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-4 mb-2">
                                                <div class="col-12 col-sm-12 col-lg-3 mb-1">
                                                    <label>Hold Down Clip</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="supply" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
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
                <div class="col-12 col-sm-12 col-md-12 col-lg-5">
                    <div class="card">
                        <div class="card-header">
                            <h4 class="card-title text-center">Information</h4>
                        </div>
                        <div class="card-content">
                            <div class="card-body">
                                <b>MATERIALS & SIZES</b>
                                <ul>
                                    <li>Aluminium 25mm Slat Only</li>
                                    <li>0.21mm Thickness</li>
                                </ul>
                                <b>OPERATING SYSTEM</b>
                                <ul>
                                    <li>Cord operation including cord lock, with wand tilter.</li>
                                </ul>
                                <b>LIMITATIONS</b>
                                <ul>
                                    <li>Minimum width : 300mm</li>
                                    <li>Maximum width : 3010mm</li>
                                    <li>Minimum drop : 300mm</li>
                                    <li>Maximum drop : 3200mm</li>
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

    <div class="modal modal-blur fade" id="modalLayout" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-body">
                    <div class="list-group list-group-horizontal-sm mb-1 text-center" role="tablist">
                        <a class="list-group-item list-group-item-action active" id="list-left-list" data-bs-toggle="list" href="#list-left" role="tab">2 on 1 Left - Left</a>
                        <a class="list-group-item list-group-item-action" id="list-right-list" data-bs-toggle="list" href="#list-right" role="tab">2 on 1 Right - Right</a>
                        <a class="list-group-item list-group-item-action" id="list-leftright-list" data-bs-toggle="list" href="#list-leftright" role="tab">2 on 1 Left - Right</a>
                    </div>

                    <div class="tab-content text-justify">
                        <div class="tab-pane fade show active" id="list-left" role="tabpanel" aria-labelledby="list-left-list">
                            <br />
                            <img runat="server" src="~/Assets/images/products/aluminium/2on1aluminiumleft.png" alt="Sub Type Image" style="max-width:100%;height:auto;">
                        </div>
                        <div class="tab-pane fade" id="list-right" role="tabpanel" aria-labelledby="list-right-list">
                            <br />
                            <img runat="server" src="~/Assets/images/products/aluminium/2on1aluminiumright.png" alt="Sub Type Image" style="max-width:100%;height:auto;">
                        </div>
                        <div class="tab-pane fade" id="list-leftright" role="tabpanel" aria-labelledby="list-leftright-list">
                            <br />
                            <img runat="server" src="~/Assets/images/products/aluminium/2on1aluminiumleftright.png" alt="Sub Type Image" style="max-width:100%;height:auto;">
                        </div>
                    </div>
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
                    <asp:Image runat="server" CssClass="w-100" ImageUrl="~/Assets/images/products/aluminium/gallery.jpg" />
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

    <script src="/Scripts/WebOrders/Aluminium.js?v=2.0.0"></script>
</asp:Content>
