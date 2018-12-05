﻿using System.Threading;
using Autofac;
using EmailSkill;
using EmailSkillTest.Flow.Fakes;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Authentication;
using Microsoft.Bot.Solutions.Dialogs;
using Microsoft.Bot.Solutions.Dialogs.BotResponseFormatters;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;

namespace EmailSkillTest.Flow
{
    public class EmailBotTestBase : BotTestBase
    {
        public IStatePropertyAccessor<EmailSkillState> EmailStateAccessor { get; set; }

        public ConversationState ConversationState { get; set; }

        public UserState UserState { get; set; }

        public IServiceManager ServiceManager { get; set; }

        public ISkillConfiguration Services { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            var builder = new ContainerBuilder();

            this.ConversationState = new ConversationState(new MemoryStorage());
            this.UserState = new UserState(new MemoryStorage());
            this.EmailStateAccessor = this.ConversationState.CreateProperty<EmailSkillState>(nameof(EmailSkillState));
            this.Services = new MockSkillConfiguration();

            builder.RegisterInstance(new BotStateSet(this.UserState, this.ConversationState));
            var fakeServiceManager = new MockServiceManager();
            builder.RegisterInstance<IServiceManager>(fakeServiceManager);

            this.Container = builder.Build();
            this.ServiceManager = fakeServiceManager;

            this.BotResponseBuilder = new BotResponseBuilder();
            this.BotResponseBuilder.AddFormatter(new TextBotResponseFormatter());
        }

        public Activity GetAuthResponse()
        {
            ProviderTokenResponse providerTokenResponse = new ProviderTokenResponse();
            providerTokenResponse.TokenResponse = new TokenResponse(token: "test");
            providerTokenResponse.AuthenticationProvider = OAuthProvider.AzureAD;
            return new Activity(ActivityTypes.Event, name: "tokens/response", value: providerTokenResponse);
        }

        public TestFlow GetTestFlow()
        {
            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(this.ConversationState));

            var testFlow = new TestFlow(adapter, async (context, token) =>
            {
                var bot = this.BuildBot() as EmailSkill.EmailSkill;

                var state = await this.EmailStateAccessor.GetAsync(context, () => new EmailSkillState());
                state.MailSourceType = MailSource.Microsoft;

                await bot.OnTurnAsync(context, CancellationToken.None);
            });

            return testFlow;
        }

        public override IBot BuildBot()
        {
            return new EmailSkill.EmailSkill(this.Services, this.ConversationState, this.UserState,  this.ServiceManager, true);
        }
    }
}
