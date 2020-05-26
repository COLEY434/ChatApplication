let createRoomBtn = document.getElementById("create-room-btn");
let createRoomModal = document.getElementById("create-room-modal");


createRoomBtn.addEventListener("click", function(){
    createRoomModal.classList.add("active")
})

var CloseModal = function CloseModal(){
   
    createRoomModal.classList.remove("active")
}
