using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Mapping;

namespace UmbCheckout.Stripe.Composers
{
    internal class MapDefinitionsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<UmbCheckoutConfigurationMappingDefinition>();
        }
    }

    public class UmbCheckoutConfigurationMappingDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<ShippingRate, UmbCheckoutStripeShipping>((_, _) => new UmbCheckoutStripeShipping(), Map);
            mapper.Define<UmbCheckoutStripeShipping, ShippingRate>((_, _) => new ShippingRate(), Map);
            mapper.Define<Models.UmbCheckoutStripeSettings, Pocos.UmbCheckoutStripeSettings>((_, _) => new Pocos.UmbCheckoutStripeSettings(), Map);
            mapper.Define<Pocos.UmbCheckoutStripeSettings, Models.UmbCheckoutStripeSettings>((_, _) => new Models.UmbCheckoutStripeSettings(), Map);
        }

        private static void Map(ShippingRate source, UmbCheckoutStripeShipping target, MapperContext context)
        {
            target.Key = source.Key;
            target.Id = source.Id;
            target.Name = source.Name;
            target.Value = source.Value;
        }

        private static void Map(UmbCheckoutStripeShipping source, ShippingRate target, MapperContext context)
        {
            target.Key = source.Key;
            target.Id = source.Id;
            target.Name = source.Name;
            target.Value = source.Value;
        }

        private static void Map(Pocos.UmbCheckoutStripeSettings source, Models.UmbCheckoutStripeSettings target, MapperContext context)
        {
            target.Key = source.Key;
            target.Id = source.Id;
            target.UseLiveApiDetails = source.UseLiveApiDetails;
            target.ShippingAllowedCountries = source.ShippingAllowedCountries;
            target.CollectPhoneNumber = source.CollectPhoneNumber;
            target.CollectPromotionalEmailsConsent = source.CollectPromotionalConsent;
        }

        private static void Map(Models.UmbCheckoutStripeSettings source, Pocos.UmbCheckoutStripeSettings target, MapperContext context)
        {
            target.Key = source.Key;
            target.Id = source.Id;
            target.UseLiveApiDetails = source.UseLiveApiDetails;
            target.ShippingAllowedCountries = source.ShippingAllowedCountries;
            target.CollectPhoneNumber = source.CollectPhoneNumber;
            target.CollectPromotionalConsent = source.CollectPromotionalEmailsConsent;
        }
    }
}
