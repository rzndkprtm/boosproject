//let choices = document.querySelectorAll('.choices');
//let initChoice;
//for (let i = 0; i < choices.length; i++) {
//    if (choices[i].classList.contains("multiple-remove")) {
//        initChoice = new Choices(choices[i],
//            {
//                delimiter: ',',
//                editItems: true,
//                maxItemCount: -1,
//                removeItemButton: true,
//            });
//    } else {
//        initChoice = new Choices(choices[i]);
//    }
//}

let choices = document.querySelectorAll('.choices');
let initChoice;

for (let i = 0; i < choices.length; i++) {
    let options = {};

    if (choices[i].classList.contains("multiple-remove")) {
        options = {
            delimiter: ',',
            editItems: true,
            maxItemCount: -1,
            removeItemButton: true
        };
    }

    if (choices[i].dataset.searchLimit) {
        options.searchResultLimit = parseInt(choices[i].dataset.searchLimit, 10);
    }

    initChoice = new Choices(choices[i], options);
}
