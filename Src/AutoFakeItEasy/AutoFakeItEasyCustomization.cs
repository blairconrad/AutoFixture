﻿using System;
using System.Globalization;
using System.Reflection;
using AutoFixture.Kernel;
using FakeItEasy;

namespace AutoFixture.AutoFakeItEasy
{
    /// <summary>
    /// Enables auto-mocking with FakeItEasy.
    /// </summary>
    public class AutoFakeItEasyCustomization : ICustomization
    {
        private bool generateDelegates;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFakeItEasyCustomization"/> class.
        /// </summary>
        public AutoFakeItEasyCustomization()
            : this(new FakeItEasyRelay())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFakeItEasyCustomization"/> class with a
        /// <see cref="FakeItEasyRelay"/>.
        /// </summary>
        /// <param name="relay">The relay.</param>
        public AutoFakeItEasyCustomization(ISpecimenBuilder relay)
        {
            this.Relay = relay ?? throw new ArgumentNullException(nameof(relay));
        }

        /// <summary>
        /// Gets the relay that will be added to <see cref="IFixture.ResidueCollectors"/> when
        /// <see cref="Customize"/> is invoked.
        /// </summary>
        /// <seealso cref="AutoFakeItEasyCustomization(ISpecimenBuilder)"/>
        public ISpecimenBuilder Relay { get; }

        /// <summary>
        /// When <c>true</c>, configures the fixture to automatically generate Fakes when a delegate is requested.
        /// </summary>
        public bool GenerateDelegates
        {
            get => generateDelegates;
            set
            {
                if (value)
                {
                    AssertFakeItEasyCanFakeDelegates();
                }

                generateDelegates = value;
            }
        }

        /// <summary>
        /// Customizes an <see cref="IFixture"/> to enable auto-mocking with FakeItEasy.
        /// </summary>
        /// <param name="fixture">The fixture upon which to enable auto-mocking.</param>
        public void Customize(IFixture fixture)
        {
            if (fixture == null) throw new ArgumentNullException(nameof(fixture));

            if (this.GenerateDelegates)
            {
                fixture.Customizations.Add(new FakeItEasyRelay(new DelegateSpecification()));
            }

            fixture.Customizations.Add(
                new FakeItEasyBuilder(
                    new MethodInvoker(
                        new FakeItEasyMethodQuery())));
            fixture.ResidueCollectors.Add(this.Relay);
        }

        private static void AssertFakeItEasyCanFakeDelegates()
        {
            var minimumFakeItEasyAssemblyVersion = new Version(1, 7, 4257, 42);
            var actualFakeItEasyAssemblyVersion = typeof(A).GetTypeInfo().Assembly.GetName().Version;
            if (actualFakeItEasyAssemblyVersion < minimumFakeItEasyAssemblyVersion)
            {
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture,
                    "Option {0} was specified, but this requires FakeItEasy version {1} or higher, and {2} was detected. Either remove the option or upgrade FakeItEasy.",
                    nameof(GenerateDelegates),
                    minimumFakeItEasyAssemblyVersion,
                    actualFakeItEasyAssemblyVersion));
            }
        }
    }
}
