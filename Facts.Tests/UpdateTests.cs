using NUnit.Framework;

namespace Facts.Tests
{
	[TestFixture]
	public class UpdateTests
	{
		class Value
		{
			public int ValueField;
			public int ValueProperty { get; set; }
			public int[] ValueArray;
		}

		[Test]
		public void testUpdateSimpleField()
		{
			var inst = new Value {ValueField = 10};

			var newInst = inst.update(i => i.ValueField, v => v + 1);

			Assert.That(newInst, Is.Not.SameAs(inst));
			Assert.That(newInst.ValueField, Is.EqualTo(inst.ValueField+1));
		}

		class ValueOf
		{
			public Value Value;
		}

		[Test]
		public void testUpdateNestedField()
		{
			var inst = new ValueOf {Value = new Value() {ValueField = 10}};

			var newInst = inst.update(i => i.Value.ValueField, v => v + 1);

			Assert.That(newInst, Is.Not.SameAs(inst));
			Assert.That(newInst.Value, Is.Not.SameAs(inst.Value));
			Assert.That(newInst.Value.ValueField, Is.EqualTo(inst.Value.ValueField+1));
		}

		[Test]
		public void testUpdateNestedProperty()
		{
			var inst = new ValueOf { Value = new Value() { ValueField = 10 } };

			var newInst = inst.update(i => i.Value.ValueProperty, v => v + 1);

			Assert.That(newInst, Is.Not.SameAs(inst));
			Assert.That(newInst.Value, Is.Not.SameAs(inst.Value));
			Assert.That(newInst.Value.ValueProperty, Is.EqualTo(inst.Value.ValueProperty + 1));
		}

		[Test]
		public void testArrayFieldMember()
		{
			var inst = new Value() {ValueArray = new int[]{10}};
			var newInst = inst.update(i => i.ValueArray[0], v => v + 1);

			Assert.That(newInst, Is.Not.SameAs(inst));
			Assert.That(newInst.ValueArray, Is.Not.SameAs(inst.ValueArray));
			Assert.That(newInst.ValueArray[0], Is.EqualTo(inst.ValueArray[0] + 1));
		}



		[Test] // FAILS, TBD
		public void testArrayFieldMemberWithIndexParameter()
		{
			var inst = new Value() { ValueArray = new int[] { 10 } };

			var index = 0;
			var newInst = inst.update(i => i.ValueArray[index], v => v + 1);

			Assert.That(newInst, Is.Not.SameAs(inst));
			Assert.That(newInst.ValueArray, Is.Not.SameAs(inst.ValueArray));
			Assert.That(newInst.ValueArray[0], Is.EqualTo(inst.ValueArray[0] + 1));
		}
	}
}
