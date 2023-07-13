document.querySelectorAll("a.song-star").forEach(a =>
	a.addEventListener("click", async evt => {
		const slug = evt.target.getAttribute("data-song-slug");
		const result = await fetch(`/star/${slug}`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' }
		});
		const starred = await result.json();
		if (starred) {
			a.classList.remove("fa-thin");
			a.classList.add("fa-solid");
		} else {
			a.classList.remove("fa-solid");
			a.classList.add("fa-thin");
		}
		evt.preventDefault();
		return false;
	})
);