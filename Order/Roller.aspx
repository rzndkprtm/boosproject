<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Roller.aspx.vb" Inherits="Order_Roller" MasterPageFile="~/Site.master" Title="Roller Order" %>

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
                                                <label>Blind Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-8 form-group">
                                                <div class="input-group">
                                                    <select id="blindtype" class="form-select"></select>
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalLayout">Info</a>
                                                </div>
                                    
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Tube Type</label>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <div class="input-group">
                                                    <select id="tubetype" class="form-select"></select>
                                                    <a class="btn btn-primary me-1" data-bs-toggle="modal" data-bs-target="#modalInfoTube">Info</a>
                                                </div>
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

                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <label>Bracket Colour</label>
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
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <select id="mounting" class="form-select"></select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divremote">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Remote Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-7 form-group">
                                                    <select id="remote" class="form-select"></select>
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

                                            <div class="row mt-3" id="divdbfront">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textdbfront"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabric">
                                                <div class="row mt-3">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictype" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolour" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row mb-3" id="divroll">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="roll" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Reverse">Reverse</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Roll');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrolposition">
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

                                            <div class="row" id="divlinkindfirst">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindfirst"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincolour">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <div class="input-group">
                                                        <select id="chaincolour" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divchainstopper">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstopper" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollength">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllength" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvalue">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalue" class="form-control" autocomplete="off" placeholder="Chain Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvalue2">
                                                    <select id="controllengthvalue2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkdepfirst">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepfirst"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtype">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtype" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divbottomcolour">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcolour" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divflatbottom">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoption" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on Back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsize">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="width" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>
                                                        <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divfirstend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <div class="row mt-3 mb-2" id="divdbback">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textdbback"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divsecond">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textsecond"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabricb">
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>

                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictypeb" class="form-select"></select>
                                                    </div>
                                                </div>
                                    
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolourb" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divrollb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="rollb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Reverse">Reverse</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Roll');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkindsecond">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindsecond"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divcontrolpositionb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlpositionb" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincolourb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <div class="input-group">
                                                        <select id="chaincolourb" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Second Chain');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divchainstopperb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstopperb" class="form-select">
                                                        <option value="">Stopper ...</option>
                                                        <option value="With Stopper">With Stopper</option>
                                                        <option value="No Stopper">No Stopper</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollengthb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthb" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvalueb">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalueb" class="form-control" autocomplete="off" placeholder="Chain Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvalueb2">
                                                    <select id="controllengthvalueb2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkdepsecond">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepsecond"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtypeb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtypeb" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Second Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divbottomcolourb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcolourb" class="form-select"></select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divflatbottomb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoptionb" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2">
                                                    <a class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Flat Option');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsizeb">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="widthb" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>
                                                        <input type="number" id="dropb" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-1">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Second Size');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divsecondend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <div class="row" id="divthird">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textthird"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabricc">
                                                <div class="row"">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictypec" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolourc" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row mb-3" id="divrollc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="rollc" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Reverse">Reverse</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Roll');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkindthird">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindthird"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrolpositionc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlpositionc" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincolourc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <div class="input-group">
                                                        <select id="chaincolourc" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Chain');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divchainstopperc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstopperc" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollengthc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthc" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvaluec">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvaluec" class="form-control" autocomplete="off" placeholder="Chain Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvaluec2">
                                                    <select id="controllengthvaluec2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkdepthird">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepthird"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtypec">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtypec" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divbottomcolourc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcolourc" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divflatbottomc">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoptionc" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsizec">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="widthc" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>
                                                        <input type="number" id="dropc" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-1">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Size');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divthirdend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <div class="row" id="divfourth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textfourth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabricd">
                                                <div class="row"">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictyped" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolourd" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divrolld">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="rolld" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Reverse">Reverse</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalGallery" onclick="return showGallery('Roll');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrolpositiond">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlpositiond" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkindfourth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindfourth"></div>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divchaincolourd">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <div class="input-group">
                                                        <select id="chaincolourd" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain');">Info</a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divchainstopperd">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstopperd" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollengthd">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthd" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvalued">
                                                    <div class="input-group">
                                                        <input type="number" id="controllengthvalued" class="form-control" autocomplete="off" placeholder="Chain Length ...." />
                                                        <span class="input-group-text">mm</span>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvalued2">
                                                    <select id="controllengthvalued2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkdepfourth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepfourth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtyped">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtyped" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divbottomcolourd">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcolourd" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divflatbottomd">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoptiond" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsized">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="widthd" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>                                                  
                                                        <input type="number" id="dropd" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-1">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Fourth Size');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divfourthend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <div class="row" id="divfifth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textfifth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divfabrice">
                                                <div class="row"">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictypee" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccoloure" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3" id="divrolle">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="rolle" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Standard">Standard</option>
                                                        <option value="Reverse">Reverse</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkindfifth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindfifth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrole">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="controlpositione" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincoloure">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-9 col-sm-9 col-lg-6 form-group">
                                                    <select id="chaincoloure" class="form-select"></select>
                                                </div>
                                                <div class="col-3 col-sm-3 col-lg-2">
                                                    <a class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Type');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divchainstoppere">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstoppere" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divcontrollengthe">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthe" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group" id="divcontrollengthvaluee">
                                                    <input type="number" id="controllengthvaluee" class="form-control" autocomplete="off" placeholder="... mm" />
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvaluee2">
                                                    <select id="controllengthvaluee2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divlinkdepfifth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepfifth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtypee">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtypee" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divbottomcoloure">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcoloure" class="form-select"></select>
                                                </div>
                                            </div>

                                            <div class="row" id="divflatbottome">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoptione" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divsizee">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="widthe" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>                                                  
                                                        <input type="number" id="drope" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-1">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Fifth Size');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divfifthend">
                                                <div class="col-12"><hr /></div>
                                            </div>
                                
                                            <div class="row" id="divsixth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textsixth"></div>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div id="divfabricf">
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Type</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                        <select id="fabrictypef" class="form-select"></select>
                                                    </div>
                                                </div>
                                    
                                                <div class="row">
                                                    <div class="col-12 col-sm-12 col-lg-3">
                                                        <label>Fabric Colour</label>
                                                    </div>
                                                    <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                        <select id="fabriccolourf" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row mb-3" id="divrollf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Roll Direction</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <select id="rollf" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Standard">Standard</option>
                                                        <option value="Reverse">Reverse</option>
                                                    </select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divlinkindsixth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkindsixth"></div>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divcontrolf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Control Position</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="controlpositionf" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Left">Left</option>
                                                        <option value="Right">Right</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row" id="divchaincolourf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Colour</label>
                                                </div>
                                                <div class="col-9 col-sm-9 col-lg-6 form-group">
                                                    <select id="chaincolourf" class="form-select"></select>
                                                </div>
                                                <div class="col-3 col-sm-3 col-lg-2">
                                                    <a class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Type');">Info</a>
                                                </div>
                                            </div>

                                            <div class="row" id="divchainstopperf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Stopper</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="chainstopperf" class="form-select"></select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divcontrollengthf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Chain Length</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="controllengthf" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Standard">Standard</option>
                                                            <option value="Custom">Custom</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Chain Length');"> ? </a>
                                                    </div>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvaluef">
                                                    <input type="number" id="controllengthvaluef" class="form-control" autocomplete="off" placeholder="... mm" />
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group" id="divcontrollengthvaluef2">
                                                    <select id="controllengthvaluef2" class="form-select">
                                                        <option value=""></option>
                                                        <option value="500">500mm</option>
                                                        <option value="750">750mm</option>
                                                        <option value="1000">1000mm</option>
                                                        <option value="1200">1200mm</option>
                                                        <option value="1500">1500mm</option>
                                                    </select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divlinkdepsixth">
                                                <div class="col-12">
                                                    <div class="divider divider-left-center">
                                                        <div class="divider-text" id="textlinkdepsixth"></div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbottomtypef">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Type</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <select id="bottomtypef" class="form-select"></select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Third Bottom');">Info</a>
                                                    </div>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divbottomcolourf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bottom Colour</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomcolourf" class="form-select"></select>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divflatbottomf">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Flat Bottom</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bottomoptionf" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Fabric on back">Fabric on Back</option>
                                                        <option value="Fabric on Front">Fabric on Front</option>
                                                    </select>
                                                </div>
                                            </div>
                                
                                            <div class="row mt-3" id="divsizef">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Size</label>
                                                </div>
                                    
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Width</span>
                                                        <input type="number" id="widthf" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>
                                    
                                                <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Drop</span>
                                                        <input type="number" id="dropf" class="form-control" autocomplete="off" placeholder="... mm" />
                                                    </div>
                                                </div>

                                                <div class="col-12 col-sm-12 col-lg-1">
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Sixth Size');">Info</a>
                                                </div>
                                            </div>
                                
                                            <div class="row" id="divsixthend">
                                                <div class="col-12"><hr /></div>
                                            </div>

                                            <div class="row mt-4" id="divtoptrack">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>White Top Track</label>
                                                </div>
                                                <div class="ccol-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="toptrack" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divspringassist">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Spring Assist</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="springassist" class="form-select">
                                                        <option value=""></option>
                                                        <option value="Yes">Yes</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbracketsize">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bracket Size</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <select id="bracketsize" class="form-select">
                                                        <option value=""></option>
                                                        <option value="40">40mm</option>
                                                        <option value="55">55mm</option>
                                                        <option value="75">75mm</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divbracketextension">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Bracket Extension</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <div class="input-group">
                                                        <select id="bracketextension" class="form-select">
                                                            <option value=""></option>
                                                            <option value="Yes">Yes</option>
                                                        </select>
                                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Bracket Extension');"> ? </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mt-3" id="divadjusting">
                                                <div class="col-12 col-sm-12 col-lg-3">
                                                    <label>Adjusting Spanner</label>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-2 form-group">
                                                    <select id="adjusting" class="form-select">
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

                                            <div id="divprinting">
                                                <input type="text" id="printing" />
                                                <input type="text" id="printingb" />
                                                <input type="text" id="printingc" />
                                                <input type="text" id="printingd" />
                                                <input type="text" id="printinge" />
                                                <input type="text" id="printingf" />
                                                <input type="text" id="printingg" />
                                                <input type="text" id="printingh" />
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
                                    <li>Minimum width : 200mm</li>
                                    <li>Minimum width for motorised : 700mm</li>
                                    <li>Maximum width : 3000mm</li>
                                    <li>Minimum drop : 600mm</li>
                                    <li>Maximum drop : 3200mm</li>
                                </ul>
                                <b>STD ROLLER COMPONENTS</b>
                                <br />
                                <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('STD Roller Components');">Read Me ...</a>
                                <br /><br />
                                <b>GR ROLLER COMPONENTS - No Chain Stopper</b>
                                <br />
                                <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('GR Roller Components');">Read Me ...</a>
                                <br /><br />
                                <b>ACMEDA ROLLER COMPONENTS</b>
                                <br />
                                <a class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Acmeda Roller Components');">Read Me ...</a>
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
                    <h5 class="modal-title white" id="titleInfo">Information</h5>
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

    <div class="modal modal-blur fade" id="modalInfoTube" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-xl modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Tube Information</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-striped mb-0">
                            <thead>
                                <tr>
                                    <th>Type</th>
                                    <th>Description</th>
                                    <th>Available Control Types</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Standard</td>
                                    <td></td>
                                    <td>Chain, Mercure, Altus</td>
                                </tr>
                                <tr>
                                    <td>Gear Reduction 38mm</td>
                                    <td>Maximum width : 1810mm</td>
                                    <td>Chain</td>
                                </tr>
                                <tr>
                                    <td>Gear Reduction 45mm</td>
                                    <td>Up to 6 SQM</td>
                                    <td>Chain, Mercure, Altus</td>
                                </tr>
                                <tr>
                                    <td>Gear Reduction 49mm</td>
                                    <td></td>
                                    <td>Chain, Mercure, Altus, Sonesse 30 WF, Alpha 1Nm WF, Alpha 2Nm WF</td>
                                </tr>
                                <tr>
                                    <td>Acmeda 43mm</td>
                                    <td>Maximum width : 1810mm</td>
                                    <td>Chain, Mercure, Altus, Sonesse 30 WF, Alpha 1Nm WF, Alpha 2Nm WF</td>
                                </tr>
                                <tr>
                                    <td>Acmeda 49mm</td>
                                    <td></td>
                                    <td>Chain, Mercure, Altus, Sonesse 30 WF, Alpha 1Nm WF, Alpha 2Nm WF</td>
                                </tr>
                                <tr>
                                    <td>Sunboss 43mm</td>
                                    <td></td>
                                    <td>Chain, Mercure, Altus, Sonesse 30 WF, Alpha 1Nm WF, Alpha 2Nm WF</td>
                                </tr>
                                <tr>
                                    <td>Sunboss 50mm</td>
                                    <td></td>
                                    <td>Chain, Mercure, Altus, Sonesse 30 WF, Alpha 1Nm WF, Alpha 2Nm WF</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalLayout" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-full modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Roller Diagram</h5>
                </div>
                <div class="modal-body">
                    <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                        <a class="list-group-item list-group-item-action active" id="link2dep" data-bs-toggle="list" href="#list-link2dep" role="tab">Link 2 Blinds Dependent</a>
                        <a class="list-group-item list-group-item-action" id="link2ind" data-bs-toggle="list" href="#list-link2ind" role="tab">Link 2 Blinds Independent</a>
                        <a class="list-group-item list-group-item-action" id="link3dep" data-bs-toggle="list" href="#list-link3dep" role="tab">Link 3 Blinds Dependent</a>
                        <a class="list-group-item list-group-item-action" id="link3ind" data-bs-toggle="list" href="#list-link3ind" role="tab">Link 3 Blinds Independent</a>
                    </div>
                    <div class="tab-content text-justify">
                        <div class="tab-pane fade show active" id="list-link2dep" role="tabpanel" aria-labelledby="link2dep">
                            <asp:Image runat="server" CssClass="mt-3 mb-3 d-block mx-auto img-fluid" ImageUrl="~/Assets/images/products/roller/linkdep2.png" />
                        </div>

                        <div class="tab-pane fade" id="list-link2ind" role="tabpanel" aria-labelledby="link2ind">
                            <asp:Image runat="server" CssClass="mt-3 mb-3 d-block mx-auto img-fluid" ImageUrl="~/Assets/images/products/roller/linkind2.png" />
                        </div>
                        <div class="tab-pane fade" id="list-link3dep" role="tabpanel" aria-labelledby="link3dep">
                            <asp:Image runat="server" CssClass="mt-3 mb-3 d-block mx-auto img-fluid" ImageUrl="~/Assets/images/products/roller/linkdep3.png" />
                        </div>
                        <div class="tab-pane fade" id="list-link3ind" role="tabpanel" aria-labelledby="link3ind">
                            <asp:Image runat="server" CssClass="mt-3 mb-3 d-block mx-auto img-fluid" ImageUrl="~/Assets/images/products/roller/linkind3.png" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    
    <script src='<%: ResolveUrl("~/Scripts/WebOrders/Roller.js?v=1.0.3") %>'></script>
</asp:Content>
