function slideToggle(t,e,o){0===t.clientHeight?j(t,e,o,!0):j(t,e,o)}function slideUp(t,e,o){j(t,e,o)}function slideDown(t,e,o){j(t,e,o,!0)}function j(t,e,o,i){void 0===e&&(e=400),void 0===i&&(i=!1),t.style.overflow="hidden",i&&(t.style.display="block");var p,l=window.getComputedStyle(t),n=parseFloat(l.getPropertyValue("height")),a=parseFloat(l.getPropertyValue("padding-top")),s=parseFloat(l.getPropertyValue("padding-bottom")),r=parseFloat(l.getPropertyValue("margin-top")),d=parseFloat(l.getPropertyValue("margin-bottom")),g=n/e,y=a/e,m=s/e,u=r/e,h=d/e;window.requestAnimationFrame(function l(x){void 0===p&&(p=x);var f=x-p;i?(t.style.height=g*f+"px",t.style.paddingTop=y*f+"px",t.style.paddingBottom=m*f+"px",t.style.marginTop=u*f+"px",t.style.marginBottom=h*f+"px"):(t.style.height=n-g*f+"px",t.style.paddingTop=a-y*f+"px",t.style.paddingBottom=s-m*f+"px",t.style.marginTop=r-u*f+"px",t.style.marginBottom=d-h*f+"px"),f>=e?(t.style.height="",t.style.paddingTop="",t.style.paddingBottom="",t.style.marginTop="",t.style.marginBottom="",t.style.overflow="",i||(t.style.display="none"),"function"==typeof o&&o()):window.requestAnimationFrame(l)})}

// Responsive burger btn onclick
//document.querySelector('.burger-btn').addEventListener('click', (e) => {
//    e.preventDefault()
//    let navbar = document.querySelector('.main-navbar')

//    slideToggle(navbar, 300)
//})

document.addEventListener('DOMContentLoaded', function () {
    const burgerBtn = document.querySelector('.burger-btn');
    if (burgerBtn) {
        burgerBtn.addEventListener('click', (e) => {
            e.preventDefault();
            const navbar = document.querySelector('.main-navbar');
            slideToggle(navbar, 300);
        });
    }
});


window.onload = () => checkWindowSize()
window.addEventListener('resize',(event) => {
    checkWindowSize()
}) 

function checkWindowSize() {
    if(window.innerWidth < 1200) listener()
    if(window.innerWidth > 1200) document.querySelector('.main-navbar').style.display = ""
}

function listener() {
    let menuItems = document.querySelectorAll('.menu-item.has-sub')

    menuItems.forEach(menuItem => {
        let menuLink = menuItem.querySelector('.menu-link')
        let submenu = menuItem.querySelector('.submenu')

        if (menuLink && submenu) {
            menuLink.addEventListener('click', (e) => {
                e.preventDefault()
                submenu.classList.toggle('active')
            })
        }
    })

    // Three level menu event listener
    let submenuItems = document.querySelectorAll('.submenu-item.has-sub')

    submenuItems.forEach(submenuItem => {
        let submenuLink = submenuItem.querySelector('.submenu-link')
        let subsubmenu = submenuItem.querySelector('.subsubmenu')

        if (submenuLink && subsubmenu) {
            submenuLink.addEventListener('click', e => {
                e.preventDefault()
                subsubmenu.classList.toggle('active')
            })
        }
    })
}
