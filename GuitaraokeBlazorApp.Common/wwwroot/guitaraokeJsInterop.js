// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function showConfirm(message) {
  return confirm(message);
}


export function initDragAndDrop() {
	let dragged;
	let id;
	let index;

	HTMLCollection.prototype.indexOf = Array.prototype.indexOf;

	document.addEventListener("dragstart", ({ target, ev }) => {
		dragged = target;
		id = target.id;
		index = target.parentNode.children.indexOf(target);
	});

	document.addEventListener("dragenter", ({ target, ev }) => {
		if (target.classList.contains("dropzone")) target.classList.add("drop-target");
		ev.preventDefault();
	});

	document.addEventListener("dragleave", ({ target, ev }) => {
		if (target.classList.contains("dropzone")) target.classList.remove("drop-target");
		ev.preventDefault();
	});

	document.addEventListener("dragover", (ev) => ev.preventDefault());

	document.addEventListener("drop", async ({ target }) => {
		if (target.classList.contains("dropzone") && target.id !== id) {
			target.classList.remove("drop-target");
			dragged.remove(dragged);
			const indexDrop = target.parentNode.children.indexOf(target);
			let offset = 0;
			if (index > indexDrop) {
				target.before(dragged);
			} else {
				offset = 1;
				target.after(dragged);
			}
			const data = {
				song: dragged.getAttribute("data-song-slug"),
				position: indexDrop + offset
			};

			console.log(data);
			console.log(index, indexDrop);
			const response = await fetch('/backstage/move', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
			location.reload();
		}
	});

}
