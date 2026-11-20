<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Quotation.aspx.vb" Inherits="Quotation" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Quotation" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .quotation-layout { row-gap: 1.25rem; margin-top: .25rem; }
        .quotation-layout > [class*="col-"] { display: flex; }
        .quotation-layout .card { width: 100%; height: 100%; border: 1px solid #e8edf3; border-radius: 14px; box-shadow: 0 5px 20px rgba(31, 45, 61, .06); overflow: hidden; }
        .quotation-layout .card-header { padding: 1.25rem 1.4rem 1rem; background: #fff; border-bottom: 1px solid #edf0f4; }
        .quotation-layout .card-title { margin-bottom: .35rem; color: #253247; font-size: 1.05rem; font-weight: 700; letter-spacing: -.0.2px; }
        .quotation-layout .card-subtitle { margin: 0; color: #8993a4; font-size: .82rem; font-weight: 400; line-height: 1.5; }
        .quotation-layout .card-body { padding: 1.25rem; }
        .quotation-layout .list-group { gap: .75rem; }
        .quotation-layout .list-group-item { padding: 1rem 1.1rem; border: 1px solid #e8edf3; border-radius: 10px !important; background: #fff; color: #344054; transition: border-color .2s ease, box-shadow .2s ease, transform .2s ease; }
        .quotation-layout .list-group-item:hover,
        .quotation-layout .list-group-item:focus {
            z-index: 1;
            border-color: #b9c9df;
            background: #fbfcfe;
            box-shadow: 0 4px 14px rgba(31, 45, 61, .07);
            transform: translateY(-1px);
            outline: none;
        }
        .quotation-layout .list-group-item h6 {
            margin: 0;
            color: #253247;
            font-size: .9rem;
            font-weight: 700;
        }
        .quotation-layout .list-group-item p {
            margin-bottom: 0;
            color: #697586;
            font-size: .85rem;
            line-height: 1.6;
            overflow-wrap: anywhere;
        }
        .quotation-layout #imgQuote {
            display: block;
            max-width: 100%;
            max-height: 150px;
            margin-top: .85rem;
            padding: .5rem;
            object-fit: contain;
            object-position: left center;
            background: #f8fafc;
            border: 1px solid #edf0f4;
            border-radius: 8px;
        }
        .quotation-layout .gallery {
            cursor: zoom-in;
            margin: 0;
        }
        .quotation-layout #imgQuoteExample {
            display: block;
            width: 100%;
            max-height: 620px;
            object-fit: contain;
            border: 1px solid #edf0f4;
            border-radius: 10px;
            background: #f8fafc;
            cursor: zoom-in;
        }
        #modalLogo .modal-content,
        #modalAddress .modal-content,
        #modalContact .modal-content,
        #modalTerms .modal-content,
        #modalImage .modal-content {
            border: 0;
            border-radius: 14px;
            box-shadow: 0 14px 45px rgba(16, 24, 40, .18);
            overflow: hidden;
        }
        #modalLogo .modal-header,
        #modalAddress .modal-header,
        #modalContact .modal-header,
        #modalTerms .modal-header,
        #modalImage .modal-header {
            padding: 1.15rem 1.35rem;
            background: #f8fafc;
            border-bottom: 1px solid #e9edf2;
        }
        #modalLogo .modal-title,
        #modalAddress .modal-title,
        #modalContact .modal-title,
        #modalTerms .modal-title,
        #modalImage .modal-title {
            color: #253247;
            font-size: 1rem;
            font-weight: 700;
        }
        #modalLogo .modal-body,
        #modalAddress .modal-body,
        #modalContact .modal-body,
        #modalTerms .modal-body,
        #modalImage .modal-body {
            padding: 1.35rem;
        }
        #modalLogo .modal-footer,
        #modalAddress .modal-footer,
        #modalContact .modal-footer,
        #modalTerms .modal-footer,
        #modalImage .modal-footer {
            padding: 1rem 1.35rem;
            background: #fbfcfd;
            border-top: 1px solid #edf0f4;
        }
        #modalLogo .form-label,
        #modalAddress .form-label,
        #modalContact .form-label,
        #modalTerms .form-label {
            margin-bottom: .45rem;
            color: #475467;
            font-size: .84rem;
            font-weight: 600;
        }
        #modalLogo .form-control,
        #modalAddress .form-control,
        #modalContact .form-control,
        #modalTerms .form-control {
            min-height: 42px;
            border-color: #dce2ea;
            border-radius: 8px;
            box-shadow: none;
        }
        #modalLogo .form-control:focus,
        #modalAddress .form-control:focus,
        #modalContact .form-control:focus,
        #modalTerms .form-control:focus {
            border-color: #8ba9d0;
            box-shadow: 0 0 0 .2rem rgba(59, 111, 174, .10);
        }
        #modalLogo .btn,
        #modalAddress .btn,
        #modalContact .btn,
        #modalTerms .btn,
        #modalImage .btn {
            min-width: 88px;
            border-radius: 8px;
            font-weight: 600;
        }
        #divError .alert,
        #divErrorLogo .alert,
        #divErrorAddress .alert,
        #divErrorContact .alert,
        #divErrorTerms .alert {
            border-radius: 9px;
        }
        @media (max-width: 767.98px) {
            .quotation-layout .card-header {
                padding: 1.1rem 1.1rem .9rem;
            }
            .quotation-layout .card-body {
                padding: 1rem;
            }
            .quotation-layout #imgQuoteExample {
                max-height: 420px;
            }
        }
    </style>
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
    <section class="row quotation-layout">
        <div class="col-12 col-sm-12 col-lg-7">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Your Account</h4>
                    <h6 class="card-subtitle">Please click the section you wish to update</h6>
                </div>
                <div class="card-content">
                    <div class="card-body">
                        <div class="list-group">
                            <a href="javascript:void(0);" class="list-group-item list-group-item-action" data-bs-toggle="modal" data-bs-target="#modalLogo">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 1 - Logo</h6>
                                </div>
                                <asp:Image runat="server" CssClass="w-100" ID="imgQuote" />
                            </a>
                            <a href="javascript:void(0);" class="list-group-item list-group-item-action" data-bs-toggle="modal" data-bs-target="#modalAddress">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 2 - Address</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pAddress"></p>
                            </a>
                            <a href="javascript:void(0);" class="list-group-item list-group-item-action" data-bs-toggle="modal" data-bs-target="#modalContact">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 3 - Contact</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pContact"></p>
                            </a>
                            <a href="javascript:void(0);" class="list-group-item list-group-item-action" data-bs-toggle="modal" data-bs-target="#modalTerms">
                                <div class="d-flex w-100 justify-content-between">
                                    <h6 class="mb-1">Section 4 - Terms & Conditions</h6>
                                </div>
                                <p class="mt-2" runat="server" id="pTerms"></p>
                            </a>
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
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                        <div class="col-12 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">State</label>
                            <asp:TextBox runat="server" ID="txtState" CssClass="form-control" placeholder="State ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
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
