using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Interactivity.Extensions;

namespace Interactivity.Pagination
{
    /// <summary>
    /// Represents a builder class for making a <see cref="LazyPaginator"/>.
    /// </summary>
    public sealed class LazyPaginatorBuilder : PaginatorBuilder
    {
        #region Fields
        /// <summary>
        /// Gets or sets the method used to load the pages of the <see cref="Paginator"/> lazily.
        /// </summary>
        public Func<int, Task<PageBuilder>> PageFactory { get; set; }

        /// <summary>
        /// Gets or sets the maximum page of the <see cref="Paginator"/>.
        /// </summary>
        public int MaxPage { get; set; }
        #endregion

        #region Build
        public override Paginator Build(int startPage = 0)
        {
            if (Emotes.Count == 0)
            {
                WithDefaultEmotes();
            }

            return new LazyPaginator(Users?.AsReadOnlyCollection() ?? throw new ArgumentNullException(nameof(Users)),
                                     Emotes?.AsReadOnlyDictionary() ?? throw new ArgumentNullException(nameof(Emotes)),
                                     CancelledEmbed?.Build() ?? throw new ArgumentNullException(nameof(CancelledEmbed)),
                                     TimeoutedEmbed?.Build(), //?? throw new ArgumentNullException(nameof(TimeoutedEmbed)),
                                     Deletion,
                                     AddPaginatorFooterAsync,
                                     startPage,
                                     MaxPage);

            async Task<Page> AddPaginatorFooterAsync(int page)
            {
                var builder = await PageFactory.Invoke(page).ConfigureAwait(false);

                return builder?.WithPaginatorFooter(Footer, page, MaxPage, Users)
                               .Build();
            }
        }
        #endregion

        #region WithValue
        public new LazyPaginatorBuilder WithUsers(params SocketUser[] users) => base.WithUsers(users) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithUsers(IEnumerable<SocketUser> users) => base.WithUsers(users) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithEmotes(Dictionary<IEmote, PaginatorAction> emotes) => base.WithEmotes(emotes) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder AddEmote(PaginatorAction action, IEmote emote) => base.AddEmote(action, emote) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithCancelledEmbed(EmbedBuilder embed) => base.WithCancelledEmbed(embed) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithTimoutedEmbed(EmbedBuilder embed) => base.WithTimoutedEmbed(embed) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithDeletion(DeletionOptions deletion) => base.WithDeletion(deletion) as LazyPaginatorBuilder;
        public new LazyPaginatorBuilder WithFooter(PaginatorFooter footer) => base.WithFooter(footer) as LazyPaginatorBuilder;
        public LazyPaginatorBuilder WithDefaultEmotes()
        {
            Emotes.Add(new Emoji("⏮"), PaginatorAction.SkipToStart);
            Emotes.Add(new Emoji("◀"), PaginatorAction.Backward);
            Emotes.Add(new Emoji("▶"), PaginatorAction.Forward);
            Emotes.Add(new Emoji("⏭"), PaginatorAction.SkipToEnd);
            Emotes.Add(new Emoji("🛑"), PaginatorAction.Exit);

            return this;
        }
        public LazyPaginatorBuilder WithPageFactory(Func<int, Task<PageBuilder>> pagefactory)
        {
            PageFactory = pagefactory;
            return this;
        }
        public LazyPaginatorBuilder WithMaxPage(int maxPage)
        {
            MaxPage = maxPage;
            return this;
        }
        #endregion
    }
}
