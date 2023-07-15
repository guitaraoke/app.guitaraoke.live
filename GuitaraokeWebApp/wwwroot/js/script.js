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
document.querySelectorAll("a.remove-song-button").forEach(a => a.addEventListener("click",
	async ({ target }) => {
		if (confirm('Are you sure you don’t want to do this one any more?')) {
			const slug = target.getAttribute("data-song-slug");
			const form = new FormData();
			form.append("slug", slug);
			fetch(`/song`,
				{
					method: 'POST',
					body: form,
				}).then(() => {
					const li = target.closest("li");
					window.setTimeout(() => li.remove(), 500);
					li.classList.add("hide");
				});
		}
	}));


