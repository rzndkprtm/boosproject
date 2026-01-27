<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Quote.aspx.vb" Inherits="Setting_Quote" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Quote Setting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <section class="row">
        <div class="col-12 col-sm-12 col-lg-7">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Your Account</h4>
                    <h6 class="card-subtitle">Please click the section you wish to update</h6>
                </div>
                <div class="card-content">
                    <div class="card-body">
                        <div class="list-group">
                            <asp:LinkButton runat="server" ID="linkLogo" CssClass="list-group-item list-group-item-action" OnClick="linkLogo_Click">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 1 - Logo</h6>
                                </div>

                                <asp:Image runat="server" CssClass="w-100" ID="imgQuote" />
                            </asp:LinkButton>

                            <asp:LinkButton runat="server" ID="linkAddress" CssClass="list-group-item list-group-item-action" OnClick="linkAddress_Click">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 2 - Address</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pAddress"></p>
                            </asp:LinkButton>

                            <asp:LinkButton runat="server" ID="linkContact" CssClass="list-group-item list-group-item-action" OnClick="linkContact_Click">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 3 - Contact</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pContact"></p>
                            </asp:LinkButton>

                            <asp:LinkButton runat="server" ID="linkTerms" CssClass="list-group-item list-group-item-action" OnClick="linkTerms_Click">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 4 - Terms & Conditions</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pTerms"></p>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 col-sm-12 col-lg-5">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Example Quotation</h4>
                    <h6 class="card-subtitle">Please click the image to clarify the function of each data section that must be completed.</h6>
                </div>
                <div class="card-content">
                    <div class="card-body">
                        <div class="row gallery" data-bs-toggle="modal" data-bs-target="#modalImage">
                            <asp:Image runat="server" ID="imgQuoteExample" CssClass="w-100" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="modal fade text-left" id="modalLogo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Quotation Logo</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorLogo">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorLogo"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Your Logo</label>
                            <asp:FileUpload runat="server" ID="fuLogo" CssClass="form-control" />
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnLogo" CssClass="btn btn-primary" Text="Submit" OnClick="btnLogo_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Quotation Address</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorAddress">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorAddress"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">State</label>
                            <asp:TextBox runat="server" ID="txtState" CssClass="form-control" placeholder="State ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" visible="false">
                        <div class="col-12 form-group">
                            <label class="form-label">Country</label>
                            <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Australia" Text="Australia"></asp:ListItem>
                                <asp:ListItem Value="Indonesia" Text="Indonesia"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddress" CssClass="btn btn-primary" Text="Submit" OnClick="btnAddress_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Quotation Contact</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorContact">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorContact"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Phone</label>
                            <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnContact" CssClass="btn btn-primary" Text="Submit" OnClick="btnContact_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalTerms" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Quotation Terms & Conditions</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorTerms">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorTerms"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Terms & Conditions</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtTerms" Height="150px" CssClass="form-control" placeholder="Terms & Conditions ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnTerms" CssClass="btn btn-primary" Text="Submit" OnClick="btnTerms_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalImage" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Quotation Example</h5>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
                <div class="modal-body">
                    <div id="Gallerycarousel" class="carousel slide carousel-fade" data-bs-ride="carousel">
                        <div class="carousel-indicators">
                            <button type="button" data-bs-target="#Gallerycarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                        </div>
                        <div class="carousel-inner">
                            <div class="carousel-item active">
                                <asp:Image runat="server" ID="imgQuoteExample2" CssClass="w-100" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblOldLogo"></asp:Label>
    </div>

    <script type="text/javascript">
        function showLogo() {
            $("#modalLogo").modal("show");
        }
        function showAddress() {
            $("#modalAddress").modal("show");
        }
        function showContact() {
            $("#modalContact").modal("show");
        }
        function showTerms() {
            $("#modalTerms").modal("show");
        }

        ["modalLogo", "modalAddress", "modalContact", "modalTerms", "modalImage"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>