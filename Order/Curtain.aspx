<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Curtain.aspx.vb" Inherits="Order_Curtain" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Curtain Order" %>

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
                                                <label>Curtain Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                <select id="blindtype" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row" style="display:none;">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Curtain Type</label>
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

                                            <div class="row mb-3" id="divmouting">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Fitting</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>

                                            <%--FIRST CURTAIN--%>
                                            <div class="row" id="divfirst">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">First Curtain</div>
                                                    </div>
                                                </div>
                                            </div>
                                        
                                            <div class="row" id="divheading">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Curtain Heading</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="heading" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Double Pinch">Double Pinch</option>
                                                            <option value="Triple Pinch">Triple Pinch</option>
                                                            <option value="Inverted Box">Inverted Box</option>
                                                            <option value="Knife">Knife</option>
                                                            <option value="S-Wave">S-Wave</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Heading');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabric">
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictype" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolour" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divtrack">
                                                <div class="row mt-2">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="tracktype" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                        <select id="trackcolour" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Draw</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                        <select id="trackdraw" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Flick Stick">Flick Stick</option>
                                                            <option value="Hand">Hand</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divstackposition">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Stack Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="stackposition" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left - OWD">Left - OWD</option>
                                                        <option value="Right - OWD">Right - OWD</option>
                                                        <option value="Centre Open">Centre Open</option>
                                                        <option value="Free Flow">Free Flow</option>
                                                    </select>
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
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Drop ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>
                                        
                                            <div class="row mt-3" id="divcontrolcolour">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlcolour" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Clear">Clear</option>
                                                        <option value="Black">Black</option>
                                                        <option value="Silver">Silver</option>
                                                        <option value="White">White</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollength">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="750">750</option>
                                                            <option value="1000">1000</option>
                                                            <option value="1250">1250</option>
                                                            <option value="1500">1500</option>
                                                            <option value="1800">1800</option>
                                                            <option value="2000">2000</option>
                                                        </select>
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divfirstend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <%--SECOND CURTAIN--%>
                                            <div class="row" id="divsecond">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text">Second Curtain</div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divheadingb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Curtain Heading</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="headingb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Double Pinch">Double Pinch</option>
                                                            <option value="Inverted Box">Inverted Box</option>
                                                            <option value="Knife">Knife</option>
                                                            <option value="S-Wave">S-Wave</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Heading');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabricb">
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabrictypeb" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolourb" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divtrackb">
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="tracktypeb" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                        <select id="trackcolourb" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Track Draw</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                        <select id="trackdrawb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Flick Stick">Flick Stick</option>
                                                            <option value="Hand">Hand</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divstackpositionb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Stack Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="stackpositionb" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left - OWD">Left - OWD</option>
                                                        <option value="Right - OWD">Right - OWD</option>
                                                        <option value="Centre Open">Centre Open</option>
                                                        <option value="Free Flow">Free Flow</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divwidthb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Width</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="widthb" class="form-control" autocomplete="off" placeholder="Width ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divdropb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Drop</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <input type="number" id="dropb" class="form-control" autocomplete="off" placeholder="Drop ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divcontrolcolourb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlcolourb" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Clear">Clear</option>
                                                        <option value="Black">Black</option>
                                                        <option value="Silver">Silver</option>
                                                        <option value="White">White</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollengthb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="750">750</option>
                                                            <option value="1000">1000</option>
                                                            <option value="1250">1250</option>
                                                            <option value="1500">1500</option>
                                                            <option value="1800">1800</option>
                                                            <option value="2000">2000</option>
                                                        </select>
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divreturnlength">
                                                <div class="row mt-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Return Length</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-text">Left</span>
                                                            <input type="number" id="returnlengthvalue" class="form-control" autocomplete="off" placeholder=".... mm" />
                                                            <span class="input-group-text">mm</span>
                                                        </div>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-text">Right</span>
                                                            <input type="number" id="returnlengthvalueb" class="form-control" autocomplete="off" placeholder=".... mm" />
                                                            <span class="input-group-text">mm</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divsecondend">
                                                <div class="col-12"><hr /></div>
                                            </div>
                                        
                                            <div class="row mt-3" id="divbottomhem">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom HEM</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomhem" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Standard">Standard</option>
                                                        <option value="Weighted">Weighted</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divtieback">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Tie Back Req</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="tieback" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Kidney Shaped">Kidney Shaped</option>
                                                            <option value="Straight">Straight</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('TieBack');">?</a>
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
                                <b>CURTAIN TRACK</b>
                                <ol>
                                    <li>
                                        Commercial Track
                                        <br />
                                        <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Commercial');">Detail Gallery</a>
                                    </li>
                                    <li class="mt-3">
                                        Styletrack
                                        <br />
                                        <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Track');">Detail Gallery</a>
                                    </li>
                                </ol>
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
                    <span id="spanInfoGallery"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <script src='<%: ResolveUrl("~/Scripts/WebOrders/Curtain.js?v=1.0.3") %>'></script>
</asp:Content>
