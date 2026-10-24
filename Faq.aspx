<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Faq.aspx.vb" Inherits="Faq" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="FAQ" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .faq-page { color: #344054; padding-bottom: 30px; }
        .faq-hero { position: relative; overflow: hidden; padding: 45px 35px; margin-bottom: 28px; border-radius: 16px; background: linear-gradient(135deg, #172b4d 0%, #244d75 100%); color: #ffffff; }
        .faq-hero::after { content: ""; position: absolute; width: 250px; height: 250px; right: -65px; top: -100px; border: 40px solid rgba(255,255,255,0.06); border-radius: 50%; pointer-events: none; }
        .faq-hero-content { position: relative; z-index: 1; max-width: 720px; }
        .faq-hero-label { display: inline-block; padding: 7px 13px; margin-bottom: 16px; border: 1px solid rgba(255,255,255,0.25); border-radius: 30px; background: rgba(255,255,255,0.08); color: #e3edf8; font-size: 12px; font-weight: 700; letter-spacing: 1.5px; text-transform: uppercase; }
        .faq-hero h1 { margin: 0 0 14px; color: #ffffff; font-size: clamp(28px, 4vw, 40px); font-weight: 800; line-height: 1.25; }
        .faq-hero p { margin: 0; color: #dce7f3; font-size: 15px; line-height: 1.9; }
        .faq-search-wrapper { position: relative; max-width: 600px; margin-top: 25px; }
        .faq-search-icon { position: absolute; top: 50%; left: 17px; transform: translateY(-50%); color: #667085; font-size: 18px; pointer-events: none; }
        .faq-search { width: 100%; height: 50px; padding: 12px 18px 12px 47px; border: 1px solid rgba(255,255,255,0.5); border-radius: 10px; outline: none; background: #ffffff; color: #344054; font-size: 14px; }
        .faq-search:focus { border-color: #9dc4ec; box-shadow: 0 0 0 3px rgba(157,196,236,0.2); }
        .faq-layout { display: grid; grid-template-columns: 250px minmax(0, 1fr); gap: 25px; align-items: start; }
        .faq-sidebar { padding: 22px; border: 1px solid #eaecf0; border-radius: 14px; background: #ffffff; }
        .faq-sidebar-title { margin-bottom: 16px; color: #172b4d; font-size: 15px; font-weight: 750; }
        .faq-category-list { display: flex; flex-direction: column; gap: 7px; }
        .faq-category { display: flex; align-items: center; gap: 11px; width: 100%; padding: 12px 13px; border: 1px solid transparent; border-radius: 8px; background: transparent; color: #667085; text-align: left; font-size: 13px; font-weight: 600; cursor: pointer; transition: all 0.2s ease; }
        .faq-category i { width: 18px; font-size: 16px; }
        .faq-category:hover { background: #f3f7fb; color: #285b89; }
        .faq-category.active { border-color: #d9e8f7; background: #edf4fb; color: #285b89; }
        .faq-content { min-width: 0; }
        .faq-section { margin-bottom: 25px; }
        .faq-section-heading { display: flex; align-items: center; gap: 12px; margin-bottom: 15px; }
        .faq-section-icon { display: flex; align-items: center; justify-content: center; flex-shrink: 0; width: 42px; height: 42px; border-radius: 11px; background: #edf4fb; color: #285b89; font-size: 19px; }
        .faq-section-heading h4 { margin: 0; color: #172b4d; font-size: 18px; font-weight: 750; }
        .faq-section-heading p { margin: 4px 0 0; color: #98a2b3; font-size: 12px; }
        .faq-accordion { overflow: hidden; border: 1px solid #eaecf0; border-radius: 12px; background: #ffffff; }
        .faq-item { border-bottom: 1px solid #eaecf0; }
        .faq-item:last-child { border-bottom: 0; }
        .faq-question { display: flex; align-items: center; justify-content: space-between; gap: 18px; width: 100%; padding: 19px 21px; border: 0; background: #ffffff; color: #344054; text-align: left; font-size: 14px; font-weight: 650; line-height: 1.7; cursor: pointer; }
        .faq-question:hover { background: #f9fafb; color: #285b89; }
        .faq-question i { flex-shrink: 0; color: #667085; font-size: 16px; transition: transform 0.2s ease; }
        .faq-question[aria-expanded="true"] { background: #f8fbfe; color: #285b89; }
        .faq-question[aria-expanded="true"] i { transform: rotate(180deg); }
        .faq-answer { padding: 0 21px 20px; background: #f8fbfe; }
        .faq-answer p { margin: 0; color: #667085; font-size: 13px; line-height: 1.9; }
        .faq-answer ul { padding-left: 20px; margin: 10px 0 0; color: #667085; font-size: 13px; line-height: 1.9; }
        .faq-empty { display: none; padding: 35px 20px; border: 1px dashed #d0d5dd; border-radius: 12px; color: #667085; text-align: center; }
        .faq-empty i { display: block; margin-bottom: 12px; font-size: 30px; color: #98a2b3; }
        .faq-empty h5 { margin-bottom: 8px; color: #344054; font-size: 16px; font-weight: 750; }
        .faq-empty p { margin: 0; font-size: 13px; }
        .faq-support { display: flex; align-items: center; justify-content: space-between; gap: 20px; margin-top: 28px; padding: 25px 28px; border: 1px solid #eaecf0; border-radius: 14px; background: #ffffff; }
        .faq-support-info { display: flex; align-items: center; gap: 16px; }
        .faq-support-icon { display: flex; align-items: center; justify-content: center; flex-shrink: 0; width: 48px; height: 48px; border-radius: 12px; background: #edf4fb; color: #285b89; font-size: 21px; }
        .faq-support h5 { margin: 0 0 5px; color: #172b4d; font-size: 15px; font-weight: 750; }
        .faq-support p { margin: 0; color: #667085; font-size: 13px; line-height: 1.7; }
        .faq-support-badge { display: inline-flex; align-items: center; gap: 7px; padding: 10px 15px; border: 1px solid #d0e8d7; border-radius: 8px; background: #f0f9f2; color: #267544; font-size: 12px; font-weight: 700; white-space: nowrap; }
        .faq-hidden { display: none !important; }
        @media (max-width: 991.98px) { .faq-layout { grid-template-columns: 1fr; gap: 20px; } .faq-sidebar { padding: 18px; } .faq-category-list { flex-direction: row; flex-wrap: wrap; } .faq-category { width: auto; padding: 10px 12px; } }
        @media (max-width: 575.98px) { .faq-hero { padding: 32px 22px; } .faq-category-list { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); } .faq-category { justify-content: flex-start; padding: 10px; font-size: 12px; } .faq-question { padding: 16px; font-size: 13px; } .faq-answer { padding: 0 16px 17px; } .faq-support { align-items: flex-start; flex-direction: column; padding: 22px; } .faq-support-badge { white-space: normal; } }
    </style>

    <div class="page-heading">
        <h3>FAQ</h3>
    </div>

    <div class="page-content faq-page">
        <section class="faq-hero">
            <div class="faq-hero-content">
                <span class="faq-hero-label">Help Centre</span>
                <h1>Frequently Asked Questions</h1>
                <p>
                    Find answers to common questions about JPM Direct,
                    our products, ordering process, manufacturing and delivery.
                    We are here to help you get the information you need.
                </p>
                <div class="faq-search-wrapper">
                    <i class="bi bi-search faq-search-icon"></i>
                    <input type="text" id="faqSearch" class="faq-search" placeholder="Search for a question or keyword..." aria-label="Search FAQ" autocomplete="off" />
                </div>
            </div>
        </section>
        <div class="faq-layout">
            <aside class="faq-sidebar">
                <div class="faq-sidebar-title">Browse by Category</div>
                <div class="faq-category-list">
                    <button type="button" class="faq-category active" data-category="all">
                        <i class="bi bi-grid"></i> All Questions
                    </button>

                    <button type="button" class="faq-category" data-category="company">
                        <i class="bi bi-building"></i> Company
                    </button>

                    <button type="button" class="faq-category" data-category="products">
                        <i class="bi bi-window"></i> Products
                    </button>

                    <button type="button" class="faq-category" data-category="ordering">
                        <i class="bi bi-cart3"></i> Ordering
                    </button>

                    <button type="button" class="faq-category" data-category="measurement">
                        <i class="bi bi-rulers"></i> Measurement
                    </button>

                    <button type="button" class="faq-category" data-category="delivery">
                        <i class="bi bi-truck"></i> Production & Delivery
                    </button>

                    <button type="button" class="faq-category" data-category="support">
                        <i class="bi bi-headset"></i> Support
                    </button>
                </div>
            </aside>

            <!-- FAQ CONTENT -->
            <div class="faq-content">

                <!-- COMPANY -->
                <section class="faq-section" data-section="company">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-building"></i></div>
                        <div>
                            <h4>About JPM Direct</h4>
                            <p>Learn more about our company and services.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="company">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-company-1">
                                What is JPM Direct?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-company-1">
                                <p>
                                    JPM Direct Pty Ltd is an Australian wholesale supplier
                                    of custom-made window furnishings. Established in
                                    February 2010, we work with our Indonesian manufacturing
                                    factory to supply quality window covering products
                                    at competitive factory-direct prices.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="company">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-company-2">
                                Who can purchase products from JPM Direct?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-company-2">
                                <p>
                                    JPM Direct focuses on wholesale supply and provides
                                    solutions for retailers who want to offer custom-made
                                    window furnishings without operating their own
                                    manufacturing facilities. Please contact our team
                                    to discuss account eligibility and purchasing access.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="company">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-company-3">
                                Why choose JPM Direct as a supplier?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-company-3">
                                <p>
                                    We combine extensive manufacturing capabilities,
                                    industry experience, quality-focused production,
                                    competitive pricing and customer support. Our services
                                    help retailers concentrate on sales while we take
                                    care of the manufacturing process.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- PRODUCTS -->
                <section class="faq-section" data-section="products">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-window"></i></div>
                        <div>
                            <h4>Products & Customisation</h4>
                            <p>Explore our product range and custom-made options.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="products">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-products-1">
                                What types of window furnishings do you offer?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-products-1">
                                <p>Our product range includes:</p>
                                <ul>
                                    <li>Timber Venetian Blinds</li>
                                    <li>Econo Wood Blinds</li>
                                    <li>Aluminium Venetian Blinds</li>
                                    <li>Roller Blinds</li>
                                    <li>Roman Blinds</li>
                                    <li>Panel Glides</li>
                                    <li>Curtains and Designer Shades</li>
                                </ul>
                                <p style="margin-top: 10px;">
                                    Product availability and customisation options may vary
                                    depending on the selected product.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="products">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-products-2">
                                Are your products custom-made?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-products-2">
                                <p>
                                    Yes. Our window furnishings are manufactured to order
                                    according to the specifications submitted with each
                                    order. Accurate measurements and correct product
                                    selections are important to ensure the finished
                                    product meets your requirements.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="products">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-products-3">
                                What materials are used in your products?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-products-3">
                                <p>
                                    Depending on the product, our materials include
                                    hardwoods, aluminium, fabrics, furniture-grade
                                    finishes and durable components. Please refer to
                                    the relevant product specifications for details
                                    about individual materials and available options.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="products">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-products-4">
                                Where can I find product specifications?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-products-4">
                                <p>
                                    Please refer to the product information and
                                    specifications provided through your available
                                    JPM Direct resources. If you need clarification
                                    about a product, contact our team before placing
                                    your order.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- ORDERING -->
                <section class="faq-section" data-section="ordering">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-cart3"></i></div>
                        <div>
                            <h4>Orders & Pricing</h4>
                            <p>Information about placing orders and pricing.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="ordering">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-ordering-1">
                                How do I place an order?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-ordering-1">
                                <p>
                                    Eligible customers can place orders through the
                                    available JPM Direct ordering system. Select the
                                    required product, enter the relevant measurements
                                    and specifications, review your selections and
                                    confirm that the order details are correct before
                                    submitting the order.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="ordering">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-ordering-2">
                                How is my order price calculated?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-ordering-2">
                                <p>
                                    Pricing depends on the selected product, dimensions,
                                    specifications, materials, options and any applicable
                                    pricing arrangements. Review the displayed price
                                    and order summary before confirming your order.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="ordering">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-ordering-3">
                                Can I change my order after submitting it?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-ordering-3">
                                <p>
                                    Custom-made orders may enter production after
                                    submission. If you need to make a change, contact
                                    our team as soon as possible and provide your
                                    order reference and the details that need updating.
                                    Changes may not be possible once manufacturing
                                    has started.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="ordering">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-ordering-4">
                                How can I check my order details?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-ordering-4">
                                <p>
                                    Check your order through the available ordering
                                    system or contact your JPM Direct representative.
                                    Please have your order reference ready so our
                                    team can assist you efficiently.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- MEASUREMENT -->
                <section class="faq-section" data-section="measurement">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-rulers"></i></div>
                        <div>
                            <h4>Measurements & Installation</h4>
                            <p>Get the measurements right before placing an order.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="measurement">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-measurement-1">
                                How do I measure my windows correctly?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-measurement-1">
                                <p>
                                    Measurement requirements vary depending on the
                                    product and whether the blind will be installed
                                    inside or outside the window recess. Follow the
                                    relevant JPM Direct measurement instructions for
                                    the selected product before entering dimensions.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="measurement">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-measurement-2">
                                Where can I find installation instructions?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-measurement-2">
                                <p>
                                    Please refer to the available measurement and
                                    installation instructions provided through
                                    JPM Direct resources. Always follow the instructions
                                    specific to your product type and installation method.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="measurement">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-measurement-3">
                                What happens if I submit incorrect measurements?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-measurement-3">
                                <p>
                                    Because products are custom-made to the specifications
                                    submitted, incorrect measurements can result in a
                                    product that does not fit as intended. Please check
                                    all measurements and selections carefully before
                                    submitting your order. Contact our team if you
                                    are unsure about the required measurements.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- PRODUCTION & DELIVERY -->
                <section class="faq-section" data-section="delivery">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-truck"></i></div>
                        <div>
                            <h4>Production & Delivery</h4>
                            <p>Learn about manufacturing and delivery arrangements.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="delivery">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-delivery-1">
                                How long does manufacturing and air freight take?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-delivery-1">
                                <p>
                                    JPM Direct's stated manufacturing and air-freight
                                    timeframe is within 7 days from the order date
                                    for delivery to our warehouses in Australia.
                                    The actual delivery date to the final destination
                                    may depend on additional transport, processing
                                    and delivery arrangements.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="delivery">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-delivery-2">
                                Does the 7-day timeframe mean delivery to my customer?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-delivery-2">
                                <p>
                                    No. The stated 7-day timeframe refers to
                                    manufacturing and air freight to JPM Direct's
                                    warehouses in Australia. Additional time may be
                                    required for processing and delivery to the final
                                    destination. Please confirm the expected delivery
                                    date with our team for your specific order.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="delivery">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-delivery-3">
                                How can I get an update on a delayed order?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-delivery-3">
                                <p>
                                    Contact your JPM Direct representative with
                                    your order reference. Our team can assist with
                                    checking the available order information and
                                    providing an update on the current status.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="delivery">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-delivery-4">
                                How are products checked before dispatch?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-delivery-4">
                                <p>
                                    Our manufacturing process includes quality
                                    control inspections. Products are checked by
                                    our quality control department as part of our
                                    commitment to maintaining high manufacturing
                                    standards.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- SUPPORT -->
                <section class="faq-section" data-section="support">
                    <div class="faq-section-heading">
                        <div class="faq-section-icon"><i class="bi bi-headset"></i></div>
                        <div>
                            <h4>Customer Support</h4>
                            <p>Find assistance for product and order enquiries.</p>
                        </div>
                    </div>

                    <div class="faq-accordion">

                        <div class="faq-item" data-category="support">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-support-1">
                                How can I get help with a product?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-support-1">
                                <p>
                                    Contact your JPM Direct representative with
                                    the product name and your question. Our team
                                    can help with available product information,
                                    specifications and technical enquiries.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="support">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-support-2">
                                Where can I find product training resources?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-support-2">
                                <p>
                                    JPM Direct provides user-friendly product
                                    training modules and supporting resources.
                                    Please check the available resources in your
                                    ordering system or contact your representative
                                    for access and guidance.
                                </p>
                            </div>
                        </div>

                        <div class="faq-item" data-category="support">
                            <button type="button" class="faq-question" aria-expanded="false" aria-controls="faq-support-3">
                                What information should I provide when requesting assistance?
                                <i class="bi bi-chevron-down"></i>
                            </button>
                            <div class="faq-answer faq-hidden" id="faq-support-3">
                                <p>
                                    To help us assist you efficiently, provide
                                    your order reference if applicable, the product
                                    name, a clear description of your question
                                    or issue, and any relevant measurements or
                                    supporting information.
                                </p>
                            </div>
                        </div>

                    </div>
                </section>

                <!-- NO RESULTS -->
                <div class="faq-empty" id="faqEmpty">
                    <i class="bi bi-search"></i>
                    <h5>No matching questions found</h5>
                    <p>Try another keyword or select a different category.</p>
                </div>

            </div>
        </div>

        <!-- SUPPORT FOOTER -->
        <section class="faq-support">
            <div class="faq-support-info">
                <div class="faq-support-icon">
                    <i class="bi bi-chat-dots"></i>
                </div>
                <div>
                    <h5>Still have questions?</h5>
                    <p>
                        Contact your JPM Direct representative for further assistance.
                    </p>
                </div>
            </div>

            <div class="faq-support-badge">
                <i class="bi bi-check-circle-fill"></i>
                We're here to help
            </div>
        </section>

    </div>

    <script>
        (function () {
            var faqSearch = document.getElementById('faqSearch');
            var categoryButtons = document.querySelectorAll('.faq-category');
            var faqSections = document.querySelectorAll('.faq-section');
            var faqItems = document.querySelectorAll('.faq-item');
            var faqEmpty = document.getElementById('faqEmpty');
            var activeCategory = 'all';

            function filterFaq() {
                var keyword = faqSearch.value.trim().toLowerCase();
                var visibleItems = 0;

                faqItems.forEach(function (item) {
                    var category = item.getAttribute('data-category');
                    var question = item.querySelector('.faq-question');
                    var answer = item.querySelector('.faq-answer');
                    var searchableText = (question.innerText + ' ' + answer.innerText).toLowerCase();
                    var categoryMatch = activeCategory === 'all' || category === activeCategory;
                    var searchMatch = keyword === '' || searchableText.indexOf(keyword) !== -1;
                    var visible = categoryMatch && searchMatch;

                    item.classList.toggle('faq-hidden', !visible);

                    if (!visible) {
                        answer.classList.add('faq-hidden');
                        question.setAttribute('aria-expanded', 'false');
                    }

                    if (visible) {
                        visibleItems++;
                    }
                });

                faqSections.forEach(function (section) {
                    var visibleSectionItems = section.querySelectorAll('.faq-item:not(.faq-hidden)').length;
                    section.classList.toggle('faq-hidden', visibleSectionItems === 0);
                });

                faqEmpty.style.display = visibleItems === 0 ? 'block' : 'none';
            }

            categoryButtons.forEach(function (button) {
                button.addEventListener('click', function () {
                    activeCategory = button.getAttribute('data-category');

                    categoryButtons.forEach(function (categoryButton) {
                        categoryButton.classList.toggle('active', categoryButton === button);
                    });

                    filterFaq();
                });
            });

            faqSearch.addEventListener('input', filterFaq);

            document.querySelectorAll('.faq-question').forEach(function (button) {
                button.addEventListener('click', function () {
                    var answer = document.getElementById(button.getAttribute('aria-controls'));
                    var isExpanded = button.getAttribute('aria-expanded') === 'true';

                    button.setAttribute('aria-expanded', isExpanded ? 'false' : 'true');
                    answer.classList.toggle('faq-hidden', isExpanded);
                });
            });

            filterFaq();
        })();
    </script>
</asp:Content>