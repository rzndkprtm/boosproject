<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Roman.aspx.vb" Inherits="Order_Roman" MasterPageFile="~/Site.master" Title="Roman Order" %>

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
                                        <div class="row" style="display:none;">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Blind Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Roman Style</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                <select id="tubetype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Control Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <select id="controltype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" style="display:none;">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Colour Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-9 form-group">
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

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Mounting</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

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

                                            <div class="row mt-3">
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

                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Drop</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Drop ....." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Valance Option</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="valanceoption" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row">
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

                                            <div class="row">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label id="controlcolourtitle"></label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <select id="controlcolour" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcharger">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Charger</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="charger" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divextensioncable">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Extension Cable</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="extensioncable" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divsupply">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Neo Box</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="supply" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollength">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label id="controllengthtitle"></label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Control Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcustomchainlength">
                                                    <select id="chainlengthvalue" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcustomcordlength">
                                                    <div class="input-group">
                                                        <input type="number" id="cordlengthvalue" class="form-control" autocomplete="off" placeholder="Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row mt-3" id="divbatten">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Batten Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="batten" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Alabaster">Alabaster</option>
                                                            <option value="Baltic">Baltic</option>
                                                            <option value="Black">Black</option>
                                                            <option value="Brown">Brown</option>
                                                            <option value="Cherry">Cherry</option>
                                                            <option value="Natural">Natural</option>
                                                            <option value="Teak">Teak</option>
                                                            <option value="White">White</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Batten Colour');"> ? </a>
                                                    </div>
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

                                            <div id="divprinting">
                                                <input type="text" id="printing" />
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
                                <b>LIMITATION</b>
                                <ul>
                                    <li>Minimum width for Chain and Reg Cord Lock : 360mm</li>
                                    <li>Minimum width for motorised : 700mm</li>
                                    <li>Maximum width : 2910mm</li>
                                    <li>Minimum drop : 600mm</li>
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
                    <asp:Image runat="server" CssClass="w-100" ImageUrl="~/Assets/images/products/roman/gallery.jpg" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <script src='<%: ResolveUrl("~/Scripts/WebOrders/Roman.js?v=1.0.1") %>'></script>
</asp:Content>
