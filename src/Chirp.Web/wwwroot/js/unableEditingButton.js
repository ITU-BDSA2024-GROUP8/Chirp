function unableEditingButton(bio){
    const bioP = document.getElementById('authorBio');
    const editButton = document.getElementById('editButton');
    const form = document.getElementsByClassName('bio-form')[0];
    const cancelButton = document.getElementById('cancel-button');
    const editInput = document.getElementById('editInput');

    editInput.value = bio;
    cancelButton.style.display = 'none';
    form.style.display = 'none';
    bioP.style.display = 'flex';
    editButton.style.display = 'flex';
}