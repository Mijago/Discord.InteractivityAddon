﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Interactivity.Pagination
{
    public sealed class StaticPaginator : Paginator
    {
        /// <summary>
        /// Gets the pages of the <see cref="Paginator"/>.
        /// </summary>
        public IReadOnlyCollection<Page> Pages { get; }

        /// <summary>
        /// Gets the maximum page of the <see cref="Paginator"/>.
        /// </summary>
        public override int MaxPageIndex => Pages.Count - 1;

        internal StaticPaginator(IReadOnlyCollection<SocketUser> users, IReadOnlyDictionary<IEmote, PaginatorAction> emotes,
            Embed cancelledEmbed, Embed timeoutedEmbed, DeletionOptions deletion,
            IReadOnlyCollection<Page> pages, int startPage)
            : base(users, emotes, cancelledEmbed, timeoutedEmbed, deletion, startPage)
        {
            Pages = pages;
        }

        public override Task<Page> GetOrLoadPageAsync(int page)
            => Task.FromResult(Pages.ElementAt(page));
    }
}
